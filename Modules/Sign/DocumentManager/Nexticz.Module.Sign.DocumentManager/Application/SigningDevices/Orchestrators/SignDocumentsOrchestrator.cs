using System.Collections.Concurrent;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Lib.Shared.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Sign.DocumentManager.Application.FileHandling;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.RevertLoadingDocumentsFiles;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.SignLoadingDocument;
using Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.EmailPublishers;
using Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.PrintingPublishers;
using Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SignalRNotifications;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.SaveSigningResult;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Models;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.CanUserManageSigningDevice;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetSigningDeviceByCode;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Orchestrators;

internal class SignDocumentsOrchestrator(
    ILogger<SignDocumentsOrchestrator> logger,
    ISender sender,
    IDocumentManagerFileHandler fileHandler,
    IDocumentManagerUnitOfWork unitOfWork,
    IClock clock,
    IDocumentSigningNotifier documentSigningNotifier,
    IDocumentManagerEmailPublisher documentManagerEmailPublisher,
    IDocumentManagerPrintingPublisher printingPublisher) : ISignDocumentsOrchestrator
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> DeviceSemaphores = new();
    private readonly List<SentLoadingDocument> _sentLoadingDocumentsToRevert = [];

    public async Task<ErrorOr<Success>> SignAsync(string signingDeviceCode, string currentUserName, string driverName, string licensePlate, 
        FileResult signatureFile, CancellationToken cancellationToken)
    {
        var upperSigningDeviceCode = signingDeviceCode.ToUpperInvariant();
        var semaphore = DeviceSemaphores.GetOrAdd(upperSigningDeviceCode, _ => new SemaphoreSlim(1, 1));
        
        var validationSucceeded = false;
        ErrorOr<SignDocumentsValidationResult> validationResult = default!;
        ErrorOr<LoadingDocument[]> loadingDocuments = default!;
        var signedAt = clock.TenantNowOffset;
        string[] depositorCodesToBeNotified = [];
        
        try
        {
            var acquired = await semaphore.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
            if (!acquired)
            {
                logger.LogWarning("[SIGN] [DocumentManager] current user: {UserName} cannot sign documents with signing device: {SigningDeviceCode}. " +
                                  "Timeout waiting to acquire lock for device.", 
                    currentUserName, upperSigningDeviceCode);
                return SigningDeviceErrors.ValidationDeviceBusyTimeoutToAcquireLockForDevice;
            }
            
            validationResult = await ValidateAsync(currentUserName, upperSigningDeviceCode, driverName, licensePlate, signatureFile, cancellationToken);
            if (validationResult.IsError)
                return validationResult.Errors;

            validationSucceeded = true;
            unitOfWork.BeginTransaction();
            
            loadingDocuments = 
                await SignDocumentsAsync(
                    validationResult.Value.SigningDevice.SentDocuments,
                    validationResult.Value.DriverName, 
                    validationResult.Value.LicensePlate,
                    validationResult.Value.SigningDevice.SentLoadingDocumentsByUserName!,
                    validationResult.Value.UserWhoSentDocumentsFullName,
                    signedAt,
                    validationResult.Value.UserWhoSentDocumentsSignatureFile,
                    validationResult.Value.DriverSignatureFile,
                    cancellationToken);
            
            if (loadingDocuments.IsError)
            {
                return await RevertLoadingDocumentsAndSaveFailedEventWhenSigningFailsAsync(
                    _sentLoadingDocumentsToRevert.ToArray(),
                    currentUserName,
                    validationResult.Value,
                    signedAt,
                    loadingDocuments,
                    depositorCodesToBeNotified,
                    cancellationToken);
            }
            
            depositorCodesToBeNotified = loadingDocuments.Value.Select(x => x.DepositorCode).Distinct().ToArray();
            
            await sender.Send(new SaveSigningResultCommand(
                validationResult.Value.SigningDevice.Id,
                validationResult.Value.SigningDevice.Code,
                CreateSigningResult(null, loadingDocuments),
                signedAt,
                validationResult.Value.SigningDevice.SentLoadingDocumentsByUserName!,
                validationResult.Value.DriverName,
                validationResult.Value.SigningDevice.SentDocuments), cancellationToken);
            
            await unitOfWork.CommitTransactionAsync(cancellationToken);
            
            logger.LogInformation(
                "[SIGN] [DocumentManager] current user: {UserName} signed loading documents with signing device: {SigningDeviceCode}.",
                currentUserName, validationResult.Value.SigningDevice.Code);
        }
        catch (Exception ex)
        {
            return await RevertLoadingDocumentsAndSaveFailedEventWhenUnexpectedFailureAsync(
                ex,
                currentUserName, 
                _sentLoadingDocumentsToRevert.ToArray(), 
                validationSucceeded, 
                validationResult, 
                signedAt, 
                upperSigningDeviceCode,
                depositorCodesToBeNotified,
                cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
        
        var documentPrintJobs = GetDocumentPrintJobs(validationResult.Value.SigningDevice.SentDocuments, loadingDocuments.Value);
        var isPrintingRequested = documentPrintJobs.Sum(x => x.CopiesCount) > 0;
        depositorCodesToBeNotified = loadingDocuments.Value.Select(x => x.DepositorCode).Distinct().ToArray();
        
        if (isPrintingRequested)
        {
            await documentSigningNotifier.NotifyDocumentsSigningSucceededAsync(validationResult.Value.SigningDevice.Code, depositorCodesToBeNotified, ReceivableNotification.DocumentsSigningSucceededWithRequestedPrinting, CancellationToken.None);
            
            try
            {
                await printingPublisher.PublishDocumentsPrintingAsync(
                    validationResult.Value.SigningDevice.Id,
                    validationResult.Value.SigningDevice.Code, 
                    validationResult.Value.SigningDevice.PrinterCode, 
                    documentPrintJobs, 
                    cancellationToken);
            
            }
            catch (Exception ex)
            {
                logger.LogError(ex, 
                    "[SIGN] [DocumentManager] unexpected error when publishing printing. Current user: {UserName}, signing device code: {SigningDeviceCode}. ErrorMessage: {ErrorMessage}.",
                    currentUserName, upperSigningDeviceCode, ex.Message);
            }
        }
        else
        {
            await documentSigningNotifier.NotifyDocumentsSigningSucceededAsync(validationResult.Value.SigningDevice.Code, depositorCodesToBeNotified, ReceivableNotification.DocumentsSigningSucceededWithoutPrinting, CancellationToken.None);
            logger.LogInformation("[SIGN] [DocumentManager] there are no documents to be sent to printer");
        }
        
        try
        {
            var documentEmailMessages = 
                ComposeDocumentEmailMessages(validationResult.Value.SigningDevice.SentDocuments, loadingDocuments.Value);
            await documentManagerEmailPublisher.PublishEmailsBasedOnConfigurationAsync(documentEmailMessages, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, 
                "[SIGN] [DocumentManager] unexpected error when publishing emails. Current user: {UserName}, signing device code: {SigningDeviceCode}. ErrorMessage: {ErrorMessage}.",
                currentUserName, upperSigningDeviceCode, ex.Message);
        }
        
        return Result.Success;
    }

    private async Task<Error> RevertLoadingDocumentsAndSaveFailedEventWhenSigningFailsAsync(
        SentLoadingDocument[] sentLoadingDocumentsToRevert,
        string currentUserName,
        SignDocumentsValidationResult validationResult,
        DateTimeOffset signedAt,
        ErrorOr<LoadingDocument[]> loadingDocumentsResult,
        string[] depositorCodesToBeNotified,
        CancellationToken cancellationToken)
    {
        await unitOfWork.RollbackTransactionAsync(cancellationToken);
        var revertedLoadingDocuments = sentLoadingDocumentsToRevert
            .Select(RevertLoadingDocumentFileJob.CreateFrom).ToArray();
        
        await sender.Send(new RevertLoadingDocumentsFilesCommand(revertedLoadingDocuments), cancellationToken);
        logger.LogWarning(
            "[SIGN] [DocumentManager] cannot sign documents. Current user: {UserName}, signing device: {SigningDeviceCode}. " +
            "ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}.",
            currentUserName, validationResult.SigningDevice.Code, loadingDocumentsResult.FirstError.Code, loadingDocumentsResult.FirstError.Description);
               
        unitOfWork.BeginTransaction();
        await sender.Send(new SaveSigningResultCommand(
            validationResult.SigningDevice.Id,
            validationResult.SigningDevice.Code,
            CreateSigningResult(null, loadingDocumentsResult),
            signedAt,
            validationResult.SigningDevice.SentLoadingDocumentsByUserName!,
            validationResult.DriverName,
            validationResult.SigningDevice.SentDocuments), cancellationToken);
        await unitOfWork.CommitTransactionAsync(cancellationToken);
         
        var error = SigningDeviceErrors.ValidationUnexpectedError(CorrelationIdProvider.Instance.GetInternalId());
        await documentSigningNotifier.NotifyDocumentsSigningFailedAsync(validationResult.SigningDevice.Code, depositorCodesToBeNotified, error.Description, CancellationToken.None);
        return error;
    }
    
    private async Task<Error> RevertLoadingDocumentsAndSaveFailedEventWhenUnexpectedFailureAsync(
        Exception ex,
        string currentUserName,
        SentLoadingDocument[] sentLoadingDocumentsToRevert,
        bool validationSucceeded,
        ErrorOr<SignDocumentsValidationResult> validationResult,
        DateTimeOffset signedAt,
        string upperSigningDeviceCode,
        string[] depositorCodesToBeNotified,
        CancellationToken cancellationToken)
    {
        logger.LogError(ex, 
            "[SIGN] [DocumentManager] unexpected error when signing documents with signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}. ErrorMessage: {ErrorMessage}.",
            currentUserName, upperSigningDeviceCode, ex.Message);
            
        await unitOfWork.RollbackTransactionAsync(cancellationToken);
        
        var revertedLoadingDocuments = sentLoadingDocumentsToRevert
            .Select(RevertLoadingDocumentFileJob.CreateFrom).ToArray();
        await sender.Send(new RevertLoadingDocumentsFilesCommand(revertedLoadingDocuments), cancellationToken);
        
        if (validationSucceeded)
        {
            unitOfWork.BeginTransaction();
            await sender.Send(new SaveSigningResultCommand(
                validationResult.Value.SigningDevice.Id,
                validationResult.Value.SigningDevice.Code,
                CreateSigningResult(ex.Message, new ErrorOr<LoadingDocument[]>()),
                signedAt,
                validationResult.Value.SigningDevice.SentLoadingDocumentsByUserName!,
                validationResult.Value.DriverName,
                validationResult.Value.SigningDevice.SentDocuments), cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
       
        var error = SigningDeviceErrors.ValidationUnexpectedError(CorrelationIdProvider.Instance.GetInternalId());
        await documentSigningNotifier.NotifyDocumentsSigningFailedAsync(upperSigningDeviceCode, depositorCodesToBeNotified, error.Description, CancellationToken.None);
        return error;
    }

    private async Task<ErrorOr<LoadingDocument[]>> SignDocumentsAsync(SentLoadingDocument[] sentLoadingDocuments, 
        string driverName, string licensePlate, string userName, string userFullName, DateTimeOffset signedAt,
        FileResult userWhoSentDocumentsSignatureFile, FileResult driverSignatureFile, 
        CancellationToken cancellationToken)
    {
        var loadingDocuments = new List<LoadingDocument>();
        foreach (var sentLoadingDocument in sentLoadingDocuments)
        {
            var loadingDocumentResult = 
                await sender.Send(
                    new SignLoadingDocumentCommand(
                        sentLoadingDocument.LoadingDocumentCode, 
                        sentLoadingDocument.ShouldAlsoSendLoadingDocument, 
                        sentLoadingDocument.DeliveryDocuments.Select(x => x.Code).ToArray(),
                        driverName, 
                        licensePlate,
                        userName,
                        userFullName,
                        signedAt,
                        userWhoSentDocumentsSignatureFile, 
                        driverSignatureFile), 
                    cancellationToken);
                
            if (loadingDocumentResult.IsError)
                return loadingDocumentResult.Errors; 
            
            _sentLoadingDocumentsToRevert.Add(sentLoadingDocument);
            loadingDocuments.Add(loadingDocumentResult.Value);
        }
        
        return loadingDocuments.ToArray();
    }

    private async Task<ErrorOr<SignDocumentsValidationResult>> ValidateAsync(
        string currentUserName, string signingDeviceCode, string driverName, string licensePlate, FileResult signatureFile,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(driverName))
        {
            logger.LogWarning(
                "[SIGN] [DocumentManager] driver name is mandatory field. Cannot sign documents. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, signingDeviceCode);
            return SigningDeviceErrors.ValidationDriverNameIsRequired;
        }

        if (string.IsNullOrWhiteSpace(licensePlate))
        {
            logger.LogWarning(
                "[SIGN] [DocumentManager] license plate is mandatory field. Cannot sign documents. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, signingDeviceCode);
            return SigningDeviceErrors.ValidationLicensePlateIsRequired;
        }
        
        var isFileValid = fileHandler.IsFileValid(signatureFile, 5* 1024 * 1024, [".jpg", ".jpeg", ".png"]);
        if (isFileValid.IsError)
        {
            logger.LogWarning(
                "[SIGN] [DocumentManager] file is not valid. Cannot sign documents. Current user: {UserName}, signing device code: {SigningDeviceCode}, " +
                ", file error code: {DomainErrorCode}, file error message: {DomainErrorMessage}.",
                currentUserName, signingDeviceCode, isFileValid.FirstError.Code, isFileValid.FirstError.Description);
            return isFileValid.Errors;
        }
        
        var user = await sender.Send(new GetUserResponseByUserNameQuery(currentUserName), cancellationToken);
        if (user.IsError)
        {
            logger.LogWarning(
                "[SIGN] [DocumentManager] current user is not present in users. Cannot sign documents with signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, signingDeviceCode);
            return SigningDeviceErrors.ValidationCurrentUserNameIsNotPresentInUsers;
        }
        
        var signingDevice = await sender.Send(new GetSigningDeviceByCodeQuery(signingDeviceCode), cancellationToken);
        if (signingDevice.IsError || signingDevice.Value.IsActive == false)
        {
            logger.LogWarning(
                "[SIGN] [DocumentManager] singing device does not exist. Cannot sign documents with signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, signingDeviceCode);
            return SigningDeviceErrors.ValidationSigningDeviceWithCodeDoesNotExist;
        }
        
        var canUserManageSigningDevice = await sender.Send(new CanUserManageSigningDeviceQuery(signingDevice.Value, user.Value), cancellationToken);
        if (!canUserManageSigningDevice)
        {
            logger.LogWarning(
                "[SIGN] [DocumentManager] cannot sign documents with signing device. Current user: {UserName} cannot manage signing device with code: {DeviceCode}.",
                currentUserName, signingDevice.Value.Code);
            return SigningDeviceErrors.ValidationCurrentUserCannotManageSigningDevice;
        }
        
        if (!signingDevice.Value.HasSentDocuments())
        {
            logger.LogWarning(
                "[SIGN] [DocumentManager] cannot sign documents with signing device. There are no files on the signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, signingDevice.Value.Code);
            return SigningDeviceErrors.ValidationThereAreNoFilesOnTheSigningDevice;
        }
        
        if (string.IsNullOrWhiteSpace(signingDevice.Value.SentLoadingDocumentsByUserName))
        {
            logger.LogWarning(
                "[SIGN] [DocumentManager] cannot sign documents with signing device. There is missing user who sent the documents. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, signingDevice.Value.Code);
            return SigningDeviceErrors.ValidationThereIsNoUserWhoSentTheDocuments;
        }
        
        foreach (var loadingDocToSend in signingDevice.Value.SentDocuments)
        {
            if (string.IsNullOrWhiteSpace(loadingDocToSend.LoadingDocumentCode))
            {
                logger.LogWarning(
                    "[SIGN] [DocumentManager] all loading document codes must be filled in. Cannot sign documents with empty loading document code. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                    currentUserName, signingDevice.Value.Code);
                return SigningDeviceErrors.ValidationYouMustFillInLoadingDocumentCodes;
            }

            if (loadingDocToSend.DeliveryDocuments.Length == 0 &&
                loadingDocToSend.ShouldAlsoSendLoadingDocument is false)
            {
                logger.LogWarning(
                    "[SIGN] [DocumentManager] you need to fill in at least one delivery document when loading document is not set for signature. Cannot send documents to signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}, loading document code: {LoadingDocumentCode}.",
                    currentUserName, signingDevice.Value.Code, loadingDocToSend.LoadingDocumentCode);
                return SigningDeviceErrors.ValidationAtLeastOneDeliveryDocumentMustBeFilledInWhenLoadingDocumentIsNotSent;
            }
        }
        
        var userSignatureWhoSentDocuments = await sender.Send(new GetUserSignatureFileQuery(signingDevice.Value.SentLoadingDocumentsByUserName), cancellationToken);
        if (userSignatureWhoSentDocuments.IsError)
        {
            logger.LogWarning(
                "[SIGN] [DocumentManager] cannot get signature for user who sent documents. Cannot sign documents with signing device. User who sent documents: {UserName}, signing device code: {SigningDeviceCode}. " +
                "ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}.",
                signingDevice.Value.SentLoadingDocumentsByUserName, signingDeviceCode,
                userSignatureWhoSentDocuments.FirstError.Code, userSignatureWhoSentDocuments.FirstError.Description);;
            return SigningDeviceErrors.ValidationUserWhoSentDocumentsToSigningDeviceDoesNotHaveSignatureFile;
        }
        
        var userWhoSentDocumentsToSigningDevice = await sender.Send(new GetUserResponseByUserNameQuery(signingDevice.Value.SentLoadingDocumentsByUserName), cancellationToken);
        if (userWhoSentDocumentsToSigningDevice.IsError)
        {
            logger.LogWarning(
                "[SIGN] [DocumentManager] user who sent documents to signing device does not exist anymore. Cannot sign documents with signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                userWhoSentDocumentsToSigningDevice.Value.UserName, signingDeviceCode);
            return SigningDeviceErrors.ValidationCurrentUserNameIsNotPresentInUsers;
        }
        
        if (string.IsNullOrWhiteSpace(userWhoSentDocumentsToSigningDevice.Value.FullName))
        {
            logger.LogWarning(
                "[SIGN] [DocumentManager] user who sent documents to signing device does not have full name filled in. Cannot sign documents with signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                userWhoSentDocumentsToSigningDevice.Value.UserName, signingDeviceCode);
            return SigningDeviceErrors.ValidationCurrentUserDoesNotHaveFullName;
        }
        
        return new SignDocumentsValidationResult(signingDevice.Value, driverName, licensePlate, userWhoSentDocumentsToSigningDevice.Value.FullName, userSignatureWhoSentDocuments.Value, signatureFile);
    }

    private static SigningResult CreateSigningResult(string? unexpectedErrorMessage, ErrorOr<LoadingDocument[]> signingDocumentsResult)
    {
        if (!string.IsNullOrWhiteSpace(unexpectedErrorMessage))
        {
            return new SigningResult(SigningStatus.FailureUnexpected, unexpectedErrorMessage);
        }
        
        if (signingDocumentsResult.IsError)
        {
            return new SigningResult(
                SigningStatus.FailureSigningDocument, 
                $"Error when signing document. Error code: {signingDocumentsResult.FirstError.Code}, error message: {signingDocumentsResult.FirstError.Description}");
        }
        
        return new SigningResult(SigningStatus.SuccessWithRequestedPrint, null);
    }
    
    private static DocumentPrintJob[] GetDocumentPrintJobs(
        SentLoadingDocument[] sentLoadingDocuments, 
        LoadingDocument[] signedLoadingDocuments)
    {
        var documentPrintJobs = new List<DocumentPrintJob>();
        foreach (var sentLoadingDocument in sentLoadingDocuments)
        {
            var loadingDocument = signedLoadingDocuments.FirstOrDefault(x => x.Code == sentLoadingDocument.LoadingDocumentCode);
            if (loadingDocument is not null && sentLoadingDocument.ShouldAlsoSendLoadingDocument)
                documentPrintJobs.Add(new DocumentPrintJob(loadingDocument.Code, null, loadingDocument.PrintCopiesCount));

            foreach (var deliveryDocumentCode in sentLoadingDocument.DeliveryDocuments.Select(x => x.Code).ToArray())
            {
                var deliveryDocument = loadingDocument?.DeliveryDocuments.FirstOrDefault(x => x.Code == deliveryDocumentCode);
                if (deliveryDocument is not null)
                    documentPrintJobs.Add(new DocumentPrintJob(deliveryDocument.LoadingDocumentCode, deliveryDocument.Code, deliveryDocument.PrintCopiesCount));
            }
        }
        
        return documentPrintJobs.ToArray();
    }

    private static DocumentEmailMessage[] ComposeDocumentEmailMessages(SentLoadingDocument[] signedDocuments, LoadingDocument[] signedLoadingDocumentsWithAllDeliveryDocuments)
    {
        var documentEmailMessages = new List<DocumentEmailMessage>();

        foreach (var signedDocument in signedDocuments)
        {
            var loadingDocument = signedLoadingDocumentsWithAllDeliveryDocuments.FirstOrDefault(x => x.Code == signedDocument.LoadingDocumentCode);
            if (loadingDocument is null)
                continue;
            
            var shouldLoadingDocumentBeSent = signedDocument.ShouldAlsoSendLoadingDocument;
            
            documentEmailMessages.Add(new DocumentEmailMessage(loadingDocument, shouldLoadingDocumentBeSent, signedDocument.DeliveryDocuments.Select(x => x.Code).ToArray()));
        }
        
        return documentEmailMessages.ToArray();
    }
    
    private record SignDocumentsValidationResult(
        SigningDevice SigningDevice,
        string DriverName,
        string LicensePlate,
        string UserWhoSentDocumentsFullName,
        FileResult UserWhoSentDocumentsSignatureFile,
        FileResult DriverSignatureFile);
}