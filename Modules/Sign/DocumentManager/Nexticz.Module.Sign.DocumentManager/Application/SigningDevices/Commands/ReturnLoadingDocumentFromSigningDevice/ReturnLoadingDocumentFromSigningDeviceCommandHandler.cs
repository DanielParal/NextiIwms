using System.Collections.Concurrent;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Lib.Shared.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.GetLoadingDocumentsByCodes;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.CanUserManageSigningDevice;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetSigningDeviceByCode;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.ReturnLoadingDocumentFromSigningDevice;

internal class ReturnLoadingDocumentFromSigningDeviceCommandHandler(
    ILogger<ReturnLoadingDocumentFromSigningDeviceCommandHandler> logger,
    IDocumentManagerUnitOfWork unitOfWork,
    ISender sender,
    ICurrentUserProvider currentUserProvider,
    IClock clock)
    : IRequestHandler<ReturnLoadingDocumentFromSigningDeviceCommand, ErrorOr<Success>>
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> DeviceSemaphores = new();
    
    public async Task<ErrorOr<Success>> Handle(ReturnLoadingDocumentFromSigningDeviceCommand request, CancellationToken cancellationToken)
    {
        var upperSigningDeviceCode = request.SigningDeviceCode.ToUpperInvariant();
        var semaphore = DeviceSemaphores.GetOrAdd(upperSigningDeviceCode, _ => new SemaphoreSlim(1, 1));
        var currentUserName = currentUserProvider.GetCurrentUser().UserName;
        
        try
        {
            var acquired = await semaphore.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
            if (!acquired)
            {
                logger.LogWarning("SIGN - DocumentManager - current user: {UserName} cannot return document from signing device:  {SigningDeviceCode}. " +
                                  "Timeout waiting to acquire lock for device.", 
                    currentUserName, request.SigningDeviceCode);
                return SigningDeviceErrors.ValidationDeviceBusyTimeoutToAcquireLockForDevice;
            }
            
            var validationResult = await ValidateAsync(currentUserName, upperSigningDeviceCode, cancellationToken);
            if (validationResult.IsError)
                return validationResult.Errors;

            await ReturnDocumentsAsync(validationResult.Value.SigningDevice, currentUserName, cancellationToken);
            
            logger.LogInformation(
                "SIGN - DocumentManager - current user: {UserName} returned documents from signing device with code: {SigningDeviceCode}.",
                currentUserName, validationResult.Value.SigningDevice.Code);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, 
                "SIGN - DocumentManager - error when returning document from signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}. ErrorMessage: {ErrorMessage}.",
                currentUserName, upperSigningDeviceCode, ex.Message);
            return SigningDeviceErrors.ValidationUnexpectedError(CorrelationIdProvider.Instance.GetInternalId());
        }
        finally
        {
            semaphore.Release();
        }
        
        return Result.Success;
    }

    private async Task ReturnDocumentsAsync(SigningDevice signingDevice, string currentUserName, CancellationToken cancellationToken)
    {
        var loadingDocumentCodes = signingDevice.SentDocuments.Select(x => x.LoadingDocumentCode).ToArray();
        var loadingDocuments = await sender.Send(new GetLoadingDocumentsByCodesQuery(loadingDocumentCodes), cancellationToken);

        foreach (var loadingDocument in loadingDocuments)
        {
            var loadingDocumentToReturn = signingDevice.SentDocuments.FirstOrDefault(x => x.LoadingDocumentCode == loadingDocument.Code);
            
            if (loadingDocumentToReturn == null)
                continue;
            
            var loadingListFromSigningDeviceReturnedEvent =
                new LoadingDocumentFromSigningDeviceReturnedEvent(
                    loadingDocument.Id,
                    loadingDocument.Code, 
                    loadingDocumentToReturn.ShouldAlsoSendLoadingDocument,
                    loadingDocumentToReturn.DeliveryDocuments.Select(x => x.Code).ToArray(),
                    signingDevice.Code,
                    clock.TenantNowOffset,
                    currentUserName);
            unitOfWork.AppendEvent(loadingDocument.Id, loadingListFromSigningDeviceReturnedEvent);
        }
        
        var documentFromSigningDeviceReturnedEvent =
            new DocumentsFromSigningDeviceReturnedEvent(
                signingDevice.Id,
                signingDevice.Code,
                clock.TenantNowOffset,
                currentUserName,
                signingDevice.SentDocuments);
        unitOfWork.AppendEvent(signingDevice.Id, documentFromSigningDeviceReturnedEvent);
    }
    
    private async Task<ErrorOr<ReturnLoadingDocumentValidationResult>> ValidateAsync(string currentUserName, string upperSigningDeviceCode,
        CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserResponseByUserNameQuery(currentUserName), cancellationToken);
        if (user.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user is not present in users. Cannot return documents from signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, upperSigningDeviceCode);
            return SigningDeviceErrors.ValidationCurrentUserNameIsNotPresentInUsers;
        }
        
        var signingDevice = await sender.Send(new GetSigningDeviceByCodeQuery(upperSigningDeviceCode), cancellationToken);
        if (signingDevice.IsError || signingDevice.Value.IsActive == false)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - singing device does not exist or is inactive. Cannot return documents from signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, upperSigningDeviceCode);
            return SigningDeviceErrors.ValidationSigningDeviceWithCodeDoesNotExist;
        }
        
        var canUserManageSigningDevice = await sender.Send(new CanUserManageSigningDeviceQuery(signingDevice.Value, user.Value), cancellationToken);

        if (!canUserManageSigningDevice)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user: {UserName} cannot manage signing device with code: {DeviceCode}. Cannot return documents from signing device.",
                currentUserName, signingDevice.Value.Code);
            return SigningDeviceErrors.ValidationCurrentUserCannotManageSigningDevice;
        }

        if (signingDevice.Value.SentDocuments.Length == 0)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - there are no files on the device. Cannot return documents from signing device. Current user: {UserName} cannot manage signing device with code: {DeviceCode}.",
                currentUserName, signingDevice.Value.Code);
            return SigningDeviceErrors.ValidationNoDocumentsOnTheDevice;
        }
        
        return new ReturnLoadingDocumentValidationResult(signingDevice.Value);
    }
    
    private record ReturnLoadingDocumentValidationResult(
        SigningDevice SigningDevice);
}