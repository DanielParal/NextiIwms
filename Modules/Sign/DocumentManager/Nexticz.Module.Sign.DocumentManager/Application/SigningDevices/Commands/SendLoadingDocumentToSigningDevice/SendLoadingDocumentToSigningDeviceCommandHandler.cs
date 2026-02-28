using System.Collections.Concurrent;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Sign.Settings.Contracts.Users;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Lib.Shared.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.CanUserManageLoadingDocument;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.GetLoadingDocumentsByCodes;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Models;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.CanUserManageSigningDevice;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetSigningDeviceByCode;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.SendLoadingDocumentToSigningDevice;

internal class SendLoadingDocumentToSigningDeviceCommandHandler(
        ILogger<SendLoadingDocumentToSigningDeviceCommand> logger,
        IDocumentManagerUnitOfWork unitOfWork,
        ISender sender,
        ICurrentUserProvider currentUserProvider,
        IClock clock
    ) : IRequestHandler<SendLoadingDocumentToSigningDeviceCommand, ErrorOr<Success>>
{
    
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> DeviceSemaphores = new();
    
    public async Task<ErrorOr<Success>> Handle(SendLoadingDocumentToSigningDeviceCommand request, CancellationToken cancellationToken)
    {
        var upperSigningDeviceCode = request.SigningDeviceCode.ToUpperInvariant();
        var semaphore = DeviceSemaphores.GetOrAdd(upperSigningDeviceCode, _ => new SemaphoreSlim(1, 1));
        var currentUserName = currentUserProvider.GetCurrentUser().UserName;
         
        try
        {
            var acquired = await semaphore.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
            if (!acquired)
            {
                logger.LogWarning("SIGN - DocumentManager - current user: {UserName} cannot send documents to signing device: {SigningDeviceCode}. " +
                                  "Timeout waiting to acquire lock for device.", 
                    currentUserName, request.SigningDeviceCode);
                return SigningDeviceErrors.ValidationDeviceBusyTimeoutToAcquireLockForDevice;
            }
            
            var validationResult = 
                await ValidateAsync(request.LoadingDocumentsToSend, currentUserName, upperSigningDeviceCode, 
                    request.DriverName, request.LicensePlate, cancellationToken);
            
            if (validationResult.IsError)
                return validationResult.Errors;
            
            await SendDocumentsAsync(validationResult.Value.SentLoadingDocuments, validationResult.Value.SigningDevice, currentUserName, cancellationToken);
            
            logger.LogInformation(
                "SIGN - DocumentManager - current user: {UserName} sent loading documents to signing device with code: {SigningDeviceCode}.",
                currentUserName, validationResult.Value.SigningDevice.Code);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, 
                "SIGN - DocumentManager - error when sending document to signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}. ErrorMessage: {ErrorMessage}.",
                currentUserName, upperSigningDeviceCode, ex.Message);
            return SigningDeviceErrors.ValidationUnexpectedError(CorrelationIdProvider.Instance.GetInternalId());
        }
        finally
        {
            semaphore.Release();
        }
        
        return Result.Success;
    }

    private async Task SendDocumentsAsync(SentLoadingDocument[] loadingDocumentsToSend, SigningDevice signingDevice, string currentUserName, CancellationToken cancellationToken)
    {
        foreach (var loadingDocToSend in loadingDocumentsToSend)
        {
            var loadingDocumentToSigningDeviceSentEvent =
                new LoadingDocumentToSigningDeviceSentEvent(
                    loadingDocToSend.LoadingDocumentId,
                    loadingDocToSend.LoadingDocumentCode,
                    loadingDocToSend.ShouldAlsoSendLoadingDocument,
                    loadingDocToSend.DeliveryDocuments.Select(x => x.Code).ToArray(),
                    signingDevice.Code,
                    clock.TenantNowOffset,
                    currentUserName);
            unitOfWork.AppendEvent(loadingDocToSend.LoadingDocumentId, loadingDocumentToSigningDeviceSentEvent);
        }
        
        var documentFromSigningDeviceReturnedEvent =
            new DocumentsToSigningDeviceSentEvent(
                signingDevice.Id,
                signingDevice.Code,
                clock.TenantNowOffset,
                currentUserName,
                loadingDocumentsToSend);
        unitOfWork.AppendEvent(signingDevice.Id, documentFromSigningDeviceReturnedEvent);
    }

    private async Task<ErrorOr<SendLoadingDocumentsValidationResult>> ValidateAsync(
        LoadingDocumentToSend[] loadingDocumentsToSend, string currentUserName, string upperSigningDeviceCode,
        string? driverName, string? licensePlate,
        CancellationToken cancellationToken)
    {
        if (loadingDocumentsToSend.Length == 0)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - you need to fill in at least one document. Cannot send documents to signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, upperSigningDeviceCode);
            return SigningDeviceErrors.ValidationNoDocumentsFilledIn;
        }

        foreach (var loadingDocToSend in loadingDocumentsToSend)
        {
            if (string.IsNullOrWhiteSpace(loadingDocToSend.LoadingDocumentCode))
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - all loading document codes must be filled in. Cannot send documents to signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                    currentUserName, upperSigningDeviceCode);
                return SigningDeviceErrors.ValidationYouMustFillInLoadingDocumentCodes;
            }

            if (loadingDocToSend.DeliveryDocumentCodes.Length == 0 &&
                loadingDocToSend.ShouldAlsoSendLoadingDocument is false)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - you need to fill in at least one delivery document when loading document is not sent. Cannot send documents to signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                    currentUserName, upperSigningDeviceCode);
                return SigningDeviceErrors.ValidationAtLeastOneDeliveryDocumentMustBeFilledInWhenLoadingDocumentIsNotSent;
            }
        }
        
        var user = await sender.Send(new GetUserResponseByUserNameQuery(currentUserName), cancellationToken);
        if (user.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user is not present in users. Cannot send documents to signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, upperSigningDeviceCode);
            return SigningDeviceErrors.ValidationCurrentUserNameIsNotPresentInUsers;
        }

        if (!user.Value.HasSignatureFile)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user does not have signature file. Cannot send documents to signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, upperSigningDeviceCode);
            return SigningDeviceErrors.ValidationCurrentUserDoesNotHaveSignatureFile;
        }
        
        if (string.IsNullOrWhiteSpace(user.Value.FullName))
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user does not have full name setup. Cannot send documents to signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, upperSigningDeviceCode);
            return SigningDeviceErrors.ValidationCurrentUserDoesNotHaveFullName;
        }
        
        var signingDevice = await sender.Send(new GetSigningDeviceByCodeQuery(upperSigningDeviceCode), cancellationToken);
        if (signingDevice.IsError || signingDevice.Value.IsActive == false)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - singing device does not exist. Cannot send documents to signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, upperSigningDeviceCode);
            return SigningDeviceErrors.ValidationSigningDeviceWithCodeDoesNotExist;
        }
        
        var canUserManageSigningDevice = await sender.Send(new CanUserManageSigningDeviceQuery(signingDevice.Value, user.Value), cancellationToken);
        if (!canUserManageSigningDevice)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user: {UserName} cannot manage signing device with code: {DeviceCode}.",
                currentUserName, signingDevice.Value.Code);
            return SigningDeviceErrors.ValidationCurrentUserCannotManageSigningDevice;
        }
        
        if (signingDevice.Value.HasSentDocuments())
        {
            logger.LogWarning(
                "SIGN - DocumentManager - there are already files on the signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, signingDevice.Value.Code);
            return SigningDeviceErrors.ValidationThereAreAlreadyFilesOnTheSigningDevice;
        }
        
        var loadingDocumentCodes = loadingDocumentsToSend.Select(x => x.LoadingDocumentCode).ToArray();
        var loadingDocuments = await sender.Send(new GetLoadingDocumentsByCodesQuery(loadingDocumentCodes), cancellationToken);

        if (loadingDocumentCodes.Length != loadingDocuments.Length)
        {
            var missingDocumentCodes = loadingDocumentCodes.Except(loadingDocuments.Select(x => x.Code)).ToArray();
            var missingDocumentsCommaSeparated = string.Join(", ", missingDocumentCodes);
            logger.LogWarning(
                "SIGN - DocumentManager - some loading documents don't exist. Current user: {UserName}, signing device code: {SigningDeviceCode}, Missing documents: {MissingDocuments}.",
                currentUserName, signingDevice.Value.Code, missingDocumentsCommaSeparated);
            return SigningDeviceErrors.ValidationSomeLoadingDocumentsDoNotExists(missingDocumentsCommaSeparated);
        }
        
        var sentLoadingDocuments = new List<SentLoadingDocument>();
        foreach (var loadingDocument in loadingDocuments)
        {
            var loadingDocumentResult = await ValidateLoadingDocumentAsync(
                loadingDocument, loadingDocumentsToSend, user.Value, signingDevice.Value.Code, driverName, licensePlate, cancellationToken);
            
            if (loadingDocumentResult.IsError)
                return loadingDocumentResult.Errors;
            
            sentLoadingDocuments.Add(loadingDocumentResult.Value);
        }

        return new SendLoadingDocumentsValidationResult(sentLoadingDocuments.ToArray(), signingDevice.Value);
    }

    private async Task<ErrorOr<SentLoadingDocument>> ValidateLoadingDocumentAsync(
        LoadingDocument loadingDocument, LoadingDocumentToSend[] loadingDocumentsToSend, UserResponse userResponse, 
        string signingDeviceCode, string? driverName, string? licensePlate,CancellationToken cancellationToken)
    {
        var loadingDocumentToSend = loadingDocumentsToSend.First(x => x.LoadingDocumentCode == loadingDocument.Code);
        
        var depositorCodes = await sender.Send(
            new GetDepositorResponsesByCodesAndGroupCodesQuery(userResponse.DepositorCodes, userResponse.DepositorGroupCodes), 
            cancellationToken);
        var canUserManageLoadingDocument = await sender.Send(new CanUserManageLoadingDocumentQuery(loadingDocument, depositorCodes.Select(dc => dc.Code).ToArray()), cancellationToken);

        if (!canUserManageLoadingDocument)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user: {UserName} cannot manage loading document with code: {LoadingDocumentCode}. SigningDeviceCode: {SigningDeviceCode}.",
                userResponse.UserName, loadingDocument.Code, signingDeviceCode);
            return SigningDeviceErrors.ValidationCurrentUserCannotManageLoadingDocument;
        };
        
        if (loadingDocumentToSend.ShouldAlsoSendLoadingDocument)
        {
            var canLoadingDocumentBeSentToSigningDevice = loadingDocument.CanLoadingDocumentBeSentToSigningDevice();

            if (canLoadingDocumentBeSentToSigningDevice.IsError)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - cannot send loading document to signing device. ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}. " +
                    "LoadingDocumentCode: {LoadingDocumentCode}, current user: {UserName}, signing device code: {SigningDeviceCode}.",
                    canLoadingDocumentBeSentToSigningDevice.FirstError.Code, canLoadingDocumentBeSentToSigningDevice.FirstError.Description,
                    loadingDocument.Code, userResponse.UserName, loadingDocument.Code);
                return canLoadingDocumentBeSentToSigningDevice.Errors;
            }
        }
        
        var canDeliveryDocumentsBeSentToSigningDevice = loadingDocument.CanDeliveryDocumentsBeSentToSigningDevice(loadingDocumentToSend.DeliveryDocumentCodes);
        if (canDeliveryDocumentsBeSentToSigningDevice.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - cannot send delivery documents to signing device. ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}. " +
                "Current user: {UserName}, loading document code: {LoadingDocumentCode}, signing device code: {SigningDeviceCode}.",
                canDeliveryDocumentsBeSentToSigningDevice.FirstError.Code, canDeliveryDocumentsBeSentToSigningDevice.FirstError.Description,
                userResponse.UserName, loadingDocument.Code, signingDeviceCode);
            return canDeliveryDocumentsBeSentToSigningDevice.Errors;
        }

        var deliveryDocumentsToSign = loadingDocument.DeliveryDocuments.Where(x => loadingDocumentToSend.DeliveryDocumentCodes.Contains(x.Code)).ToArray();
        return new SentLoadingDocument(
            loadingDocument.Id,
            loadingDocumentToSend.LoadingDocumentCode,
            string.IsNullOrWhiteSpace(licensePlate) ? loadingDocument.OriginalLicensePlate : licensePlate,
            string.IsNullOrWhiteSpace(driverName) ? loadingDocument.OriginalDriverName : driverName,
            loadingDocument.Weight,
            loadingDocument.AdrPoints,
            deliveryDocumentsToSign.Select(x => new SentDeliveryDocument(x.Code, x.PartnersOrderNumber)).ToArray(), 
            loadingDocumentToSend.ShouldAlsoSendLoadingDocument);
    }
    
    private record SendLoadingDocumentsValidationResult(
        SentLoadingDocument[] SentLoadingDocuments, SigningDevice SigningDevice);
}