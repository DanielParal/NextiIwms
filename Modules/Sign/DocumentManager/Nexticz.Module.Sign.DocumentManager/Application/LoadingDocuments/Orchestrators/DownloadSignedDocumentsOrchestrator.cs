
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentManager.Application.FileHandling;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.DownloadSignedDocument;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.CanUserManageLoadingDocument;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.GetLoadingDocumentsByCodes;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Orchestrators;

internal class DownloadSignedDocumentsOrchestrator(
    ILogger<DownloadSignedDocumentsOrchestrator> logger,
    ISender sender,
    ICurrentUserProvider currentUserProvider,
    IDocumentManagerFileHandler fileHandler,
    IClock clock) : IDownloadSignedDocumentsOrchestrator
{
    public async Task<ErrorOr<FileResult>> OrchestrateAsync(DownloadDocumentJobContract[] downloadDocumentJobs, CancellationToken cancellationToken)
    {
        var currentUserName = currentUserProvider.GetCurrentUser().UserName;
        var validationResult = await ValidateAsync(currentUserName, downloadDocumentJobs, cancellationToken);
        if (validationResult.IsError)
            return validationResult.Errors;

        if (validationResult.Value.Length == 0)
            return LoadingDocumentErrors.ValidationNoRequestedDocumentsForDownload;
        
        if (validationResult.Value.Length == 1)
        {
            var downloadJob = validationResult.Value[0];
            return await sender.Send(new DownloadSignedDocumentCommand(
                downloadJob.LoadingDocumentId,
                downloadJob.LoadingDocumentCode, 
                downloadJob.DeliveryDocumentCode,
                clock.UtcNowOffset,
                currentUserName), cancellationToken);
        }

        var fileResults = new List<FileResult>();
        foreach (var downloadJob in validationResult.Value)
        {
            var fileResult = await sender.Send(new DownloadSignedDocumentCommand(
                downloadJob.LoadingDocumentId,
                downloadJob.LoadingDocumentCode, 
                downloadJob.DeliveryDocumentCode,
                clock.UtcNowOffset,
                currentUserName), cancellationToken);
            
            if (fileResult.IsError)
                continue;
            
            fileResults.Add(fileResult.Value);
        }
        
        return await fileHandler.CreateZipFromFilesAsync(fileResults, "signed-documents.zip", cancellationToken);
    }

    private async Task<ErrorOr<DownloadJob[]>> ValidateAsync(
        string currentUserName,
        DownloadDocumentJobContract[] downloadDocumentJobs,
        CancellationToken cancellationToken)
    {
        if (downloadDocumentJobs.Length == 0)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - there are no requested documents for download. Cannot download document. Current user: {UserName}.",
                currentUserName);
            return LoadingDocumentErrors.ValidationNoRequestedDocumentsForDownload;
        }
        
        var user = await sender.Send(new GetUserResponseByUserNameQuery(currentUserName), cancellationToken);
        if (user.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user is not present in users. Cannot download document. Current user: {UserName}.",
                currentUserName);
            return LoadingDocumentErrors.ValidationCurrentUserNameIsNotPresentInUsers;
        }

        var depositorCodes = await sender.Send(
            new GetDepositorResponsesByCodesAndGroupCodesQuery(user.Value.DepositorCodes,
                user.Value.DepositorGroupCodes),
            cancellationToken);


        var loadingDocumentCodes = downloadDocumentJobs
            .Select(x => x.LoadingDocumentCode)
            .Distinct()
            .ToArray();
        var loadingDocuments = await sender.Send(new GetLoadingDocumentsByCodesQuery(loadingDocumentCodes), cancellationToken);

        var downloadJobsToReturn = new List<DownloadJob>();
        foreach (var downloadJob in downloadDocumentJobs)
        {
            var loadingDocument = loadingDocuments.FirstOrDefault(x => x.Code == downloadJob.LoadingDocumentCode);
            if (loadingDocument is null)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - object {ObjectName} does not exist. Cannot download document. Current user: {UserName}, loading document code: {LoadingDocumentCode}, delivery document code: {DeliveryDocumentCode}.",
                    nameof(LoadingDocument), currentUserName, downloadJob.LoadingDocumentCode,
                    downloadJob.DeliveryDocumentCode);
                return LoadingDocumentErrors.ValidationLoadingDocumentDoesNotExist;
            }

            var canDocumentBeDownloaded = loadingDocument.CanDocumentBeDownloaded(downloadJob.DeliveryDocumentCode);
            if (canDocumentBeDownloaded.IsError)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - object {ObjectName} cannot be downloaded. Current user: {UserName}, loading document code: {LoadingDocumentCode}, " +
                    "delivery document code: {DeliveryDocumentCode}, Domain error code: {DomainErrorCode}, Domain error message: {DomainErrorMessage}.",
                    nameof(LoadingDocument), currentUserName, downloadJob.LoadingDocumentCode, downloadJob.DeliveryDocumentCode,
                    canDocumentBeDownloaded.FirstError.Code, canDocumentBeDownloaded.FirstError.Description);
                return canDocumentBeDownloaded.Errors;
            }
            
            var canUserManageLoadingDocument =
                await sender.Send(
                    new CanUserManageLoadingDocumentQuery(loadingDocument,
                        depositorCodes.Select(dc => dc.Code).ToArray()), cancellationToken);
            if (!canUserManageLoadingDocument)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - current user: {UserName} cannot manage loading document with code: {LoadingDocumentCode}. We cannot download document.",
                    user.Value.UserName, loadingDocument.Code);
                return LoadingDocumentErrors.ValidationCurrentUserCannotManageLoadingDocument;
            }
            
            downloadJobsToReturn.Add(new DownloadJob(loadingDocument.Id, downloadJob.LoadingDocumentCode, downloadJob.DeliveryDocumentCode));
        }
        
        return downloadJobsToReturn.ToArray();
    }
    
    
    private record DownloadJob(Guid LoadingDocumentId, string LoadingDocumentCode, string? DeliveryDocumentCode);
}