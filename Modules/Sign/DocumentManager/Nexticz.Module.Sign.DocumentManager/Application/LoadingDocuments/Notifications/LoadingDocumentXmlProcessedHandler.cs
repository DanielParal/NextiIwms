using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentLoader.Contracts;
using Nexticz.Module.Sign.DocumentLoader.Contracts.Notifications;
using Nexticz.Module.Sign.Settings.Contracts.Constants.Queries;
using Nexticz.Module.Sign.Settings.Contracts.DeliveryMethods.Queries;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.DocumentManager.Application.FileHandling;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.CreateLoadingDocument;
using Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.EmailPublishers;
using Nexticz.Module.Sign.DocumentManager.Application.PartnersAndReceivers.Commands;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Notifications;

internal class LoadingDocumentXmlProcessedHandler(
    IDocumentManagerUnitOfWork unitOfWork,
    ILogger<IDocumentManagerUnitOfWork> logger,
    IMediator sender,
    IDocumentManagerFileHandler documentManagerFileHandler,
    IDocumentManagerEmailPublisher documentManagerEmailPublisher) : INotificationHandler<LoadingDocumentXmlProcessedNotification>
{
    public async Task Handle(LoadingDocumentXmlProcessedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("SIGN - DocumentManager - loading list xml processes notification received. LoadingDocumentCode: {LoadingDocumentCode}.", 
            notification.LoadingDocument.Code);
        
        await CheckMissingDepositorAndDeliveryMethodsAsync(notification.LoadingDocument, cancellationToken);
        
        try
        {
            await sender.Send(new CreateMissingPartnersAndReceiversCommand(notification.LoadingDocument.DeliveryNotes), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "SIGN - DocumentManager - error while creating new partner or receiver. LoadingDocumentCode: {LoadingDocumentCode}. ErrorMessage: {ErrorMessage}",
                notification.LoadingDocument.Code, ex.Message);
            // continue even if it fails to create a new partner or receiver - most important is to transfer documents into the DocumentManager module
        }
        
        try
        {
            unitOfWork.BeginTransaction();
            
            var loadingListCreated = await sender.Send(
                new CreateLoadingDocumentCommand(notification.LoadingDocument), cancellationToken);

            if (loadingListCreated.IsError)
            {
                logger.LogWarning("SIGN - DocumentManager - loading list was not saved into the db. Rolling back. LoadingDocumentCode: {LoadingDocumentCode}.", 
                    notification.LoadingDocument.Code);
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return;
            }
            
            await documentManagerFileHandler.CopyLoadingListFromLoaderFolderToManagerFolderAsync(loadingListCreated.Value, cancellationToken);

            await unitOfWork.CommitTransactionAsync(cancellationToken);
            
            logger.LogInformation("SIGN - DocumentManager - loading list was saved into db and files were moved into DocumentManager folder. LoadingDocumentCode: {LoadingDocumentCode}.", 
                notification.LoadingDocument.Code);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, 
                "SIGN - DocumentManager - error while processing data from loading list xml with code: {LoadingDocumentCode}. ErrorMessage: {ErrorMessage}", 
                notification.LoadingDocument.Code, ex.Message);
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
        }
    }

    private async Task CheckMissingDepositorAndDeliveryMethodsAsync(LoadingDocumentContract loadingDocumentContract,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!await DoesDepositorExistAsync(loadingDocumentContract.DepositorCode, cancellationToken))
            {
                await NotifyMissingDepositorsAsync(
                    loadingDocumentContract.DepositorCode, loadingDocumentContract.Code, loadingDocumentContract.LoadingInWmsFinishedBy, cancellationToken);
            }
            
            var missingDeliveryMethods = await GetMissingDeliveryMethodsAsync(loadingDocumentContract, cancellationToken);
            if (missingDeliveryMethods.Length > 0)
            {
                await NotifyMissingDeliveryMethodsAsync(missingDeliveryMethods, loadingDocumentContract.Code, loadingDocumentContract.LoadingInWmsFinishedBy, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "SIGN - DocumentManager - error while checking missing data. LoadingDocumentCode: {LoadingDocumentCode}. ErrorMessage: {ErrorMessage}",
                loadingDocumentContract.Code, ex.Message);
            // continue even if it fails to check data - most important is to transfer documents into the DocumentManager module
        }
    }

    private async Task<string[]> GetMissingDeliveryMethodsAsync(LoadingDocumentContract loadingDocumentContract,
        CancellationToken cancellationToken)
    {
        var nlDeliveryMethodCode = loadingDocumentContract.DeliveryMethodCode;
        var dlDeliveryMethodCodes = loadingDocumentContract.DeliveryNotes.Select(dn => dn.DeliveryMethodCode);
        var distinctDeliveryMethodCodes = dlDeliveryMethodCodes.Union([nlDeliveryMethodCode]).Distinct().ToArray();
        
        var existingDeliveryMethods = await sender.Send(new GetDeliveryMethodContractsByCodesQuery(distinctDeliveryMethodCodes), cancellationToken);

        return distinctDeliveryMethodCodes.Except(existingDeliveryMethods.Select(dm => dm.Code)).ToArray();
    }

    private async Task NotifyMissingDeliveryMethodsAsync(string[] missingDeliveryMethodCodes, string loadingDocumentCode, string loadingInWmsFinishedBy,
        CancellationToken cancellationToken)
    {
        logger.LogWarning("SIGN - DocumentManager - missing deliveryMethods: {DeliveryMethodCodes} for loading document: {LoadingDocumentCode}", missingDeliveryMethodCodes, loadingDocumentCode);
        var emailsToNotify =
            await sender.Send(new GetEmailsForMissingRequiredDataDuringTransferConstantValueQuery(), cancellationToken);
        if (emailsToNotify.Length == 0)
        {
            logger.LogWarning("SIGN - DocumentManager - no emails to notify. Cannot send email notification");
            return;
        }
        
        await documentManagerEmailPublisher.PublishEmailsAboutMissingDeliveryMethodAsync(emailsToNotify,
            missingDeliveryMethodCodes, loadingDocumentCode,
            loadingInWmsFinishedBy, cancellationToken);
    }

    private async Task<bool> DoesDepositorExistAsync(string depositorCode, CancellationToken cancellationToken)
    {
        var existingDepositor = await sender.Send(new GetDepositorResponseByCodeQuery(depositorCode), cancellationToken);
        return existingDepositor.HasValue();
    }
    
    private async Task NotifyMissingDepositorsAsync(string depositorCode, string loadingDocumentCode, string loadingInWmsFinishedBy, CancellationToken cancellationToken)
    {
        logger.LogWarning("SIGN - DocumentManager - missing depositor: {DepositorCode} for loading document: {LoadingDocumentCode}", depositorCode, loadingDocumentCode);
        var emailsToNotify =
            await sender.Send(new GetEmailsForMissingRequiredDataDuringTransferConstantValueQuery(), cancellationToken);
        if (emailsToNotify.Length == 0)
        {
            logger.LogWarning("SIGN - DocumentManager - no emails to notify. Cannot send email notification");
            return;
        }
        
        await documentManagerEmailPublisher.PublishEmailsAboutMissingDepositorAsync(emailsToNotify,
            depositorCode, loadingDocumentCode,
            loadingInWmsFinishedBy, cancellationToken);
    }
}