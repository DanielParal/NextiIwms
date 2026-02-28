using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.DeleteLoadingDocument;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.RevertLoadingDocumentsFiles;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.CanUserManageLoadingDocument;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.GetLoadingDocumentsByCodes;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Orchestrators;

internal class DeleteDocumentsOrchestrator(
    ILogger<DeleteDocumentsOrchestrator> logger,
    ISender sender,
    ICurrentUserProvider currentUserProvider,
    IDocumentManagerUnitOfWork unitOfWork,
    IClock clock) : IDeleteDocumentsOrchestrator
{
    
    private readonly List<RevertLoadingDocumentFileJob> _revertLoadingDocumentFileJobs = [];
    
    public async Task<ErrorOr<Success>> OrchestrateAsync(string deleteReason, DeleteDocumentJobContract[] deleteDocumentJobs, CancellationToken cancellationToken)
    {
        var currentUserName = currentUserProvider.GetCurrentUser().UserName;
        var validationResult = await ValidateAsync(currentUserName, deleteReason, deleteDocumentJobs, cancellationToken);
        
        if (validationResult.IsError)
            return validationResult.Errors;
        
        if (validationResult.Value.DeleteJobs.Length == 0)
            return LoadingDocumentErrors.ValidationNoRequestedDocumentsForDownload;

        var deletedAt = clock.TenantNowOffset;
        unitOfWork.BeginTransaction();
        foreach (var deleteJob in validationResult.Value.DeleteJobs)
        {
            var result = 
                await sender.Send(new DeleteLoadingDocumentCommand(
                    deleteJob.LoadingDocumentId,
                    deleteJob.LoadingDocumentCode,
                    deleteJob.ShouldDeleteLoadingDocument,
                    deleteJob.DeliveryDocumentCodes, 
                    deleteReason,
                    deletedAt,
                    currentUserName,
                    validationResult.Value.CurrentUserFullName), cancellationToken);

            if (result.IsError)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - current user: {UserName} cannot delete loading documents: {LoadingDocuments} because of error. " +
                    "ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}.",
                    currentUserName, deleteDocumentJobs, result.FirstError.Code, result.FirstError.Description);
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                await sender.Send(new RevertLoadingDocumentsFilesCommand(_revertLoadingDocumentFileJobs.ToArray()), cancellationToken);
                return result.Errors;
            }
            
            _revertLoadingDocumentFileJobs.Add(
                new RevertLoadingDocumentFileJob(deleteJob.LoadingDocumentCode, deleteJob.ShouldDeleteLoadingDocument, deleteJob.DeliveryDocumentCodes));;
        }
        
        await unitOfWork.CommitTransactionAsync(cancellationToken);
            
        logger.LogInformation(
            "SIGN - DocumentManager - current user: {UserName} deleted loading documents: {LoadingDocuments}.",
            currentUserName, deleteDocumentJobs);

        return Result.Success;
    }

    private async Task<ErrorOr<ValidationResult>> ValidateAsync(
        string currentUserName,
        string deleteReason,
        DeleteDocumentJobContract[] deleteDocumentJobs,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(deleteReason))
        {
            logger.LogWarning("SIGN - DocumentManager - there is no reason for deletion");
            return LoadingDocumentErrors.ValidationNoReasonForDeletion;
        }
        
        if (deleteDocumentJobs.Length == 0)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - there are no requested documents for delete. Cannot delete document. Current user: {UserName}.",
                currentUserName);
            return LoadingDocumentErrors.ValidationNoRequestedDocumentsForDelete;
        }
        
        var user = await sender.Send(new GetUserResponseByUserNameQuery(currentUserName), cancellationToken);
        if (user.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user is not present in users. Cannot delete document. Current user: {UserName}.",
                currentUserName);
            return LoadingDocumentErrors.ValidationCurrentUserNameIsNotPresentInUsers;
        }
        
        var depositorCodes = await sender.Send(
            new GetDepositorResponsesByCodesAndGroupCodesQuery(user.Value.DepositorCodes,
                user.Value.DepositorGroupCodes),
            cancellationToken);
        
        var loadingDocumentDistinctCodes = deleteDocumentJobs
            .Select(x => x.LoadingDocumentCode)
            .Distinct()
            .ToArray();
        var loadingDocuments = await sender.Send(new GetLoadingDocumentsByCodesQuery(loadingDocumentDistinctCodes), cancellationToken);

        var deleteJobs = ComposeDeleteJobs(deleteDocumentJobs, loadingDocuments);
        
        var deleteJobsToReturn = new List<DeleteJob>();
        foreach (var deleteJob in deleteJobs)
        {
            var loadingDocument = loadingDocuments.FirstOrDefault(x => x.Code == deleteJob.LoadingDocumentCode);
            if (loadingDocument is null)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - object {ObjectName} does not exist. Cannot delete document. Current user: {UserName}, loading document code: {LoadingDocumentCode}, delivery document codes: {DeliveryDocumentCodes}.",
                    nameof(LoadingDocument), currentUserName, deleteJob.LoadingDocumentCode, deleteJob.DeliveryDocumentCodes);
                return LoadingDocumentErrors.ValidationThisLoadingDocumentDoesNotExist(deleteJob.LoadingDocumentCode);
            }
            
            var canBeDeleted = loadingDocument.CanLoadingDocumentWithDeliveryDocumentsBeDeleted(deleteJob.ShouldDeleteLoadingDocument, deleteJob.DeliveryDocumentCodes);
            if (canBeDeleted.IsError)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - object {ObjectName} cannot be deleted. Current user: {UserName}, " +
                    "loading document code: {LoadingDocumentCode}, delivery document codes: {DeliveryDocumentCodes}. " +
                    "ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}.",
                    nameof(LoadingDocument), currentUserName, deleteJob.LoadingDocumentCode,
                    deleteJob.DeliveryDocumentCodes, canBeDeleted.FirstError.Code, canBeDeleted.FirstError.Description
                );
                return canBeDeleted.Errors;           
            }
            
            var canUserManageLoadingDocument =
                await sender.Send(
                    new CanUserManageLoadingDocumentQuery(loadingDocument,
                        depositorCodes.Select(dc => dc.Code).ToArray()), cancellationToken);
            if (!canUserManageLoadingDocument)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - current user: {UserName} cannot manage loading document with code: {LoadingDocumentCode}. We cannot delete document.",
                    user.Value.UserName, loadingDocument.Code);
                return LoadingDocumentErrors.ValidationCurrentUserCannotManageLoadingDocument;
            }
            
            deleteJobsToReturn.Add(deleteJob);
        } 
        
        return new ValidationResult(user.Value.FullName, deleteJobsToReturn.ToArray());
    }

    private static DeleteJob[] ComposeDeleteJobs(DeleteDocumentJobContract[] deleteDocumentJobs, LoadingDocument[] loadingDocuments)
    {
        var groupedDocuments = deleteDocumentJobs
            .GroupBy(x => x.LoadingDocumentCode);
        
        var deleteJobs = new List<DeleteJob>();
        foreach (var groupedDocument in groupedDocuments)
        {
            var loadingDocumentShouldBeDeleted = groupedDocument
                .FirstOrDefault(x => string.IsNullOrWhiteSpace(x.DeliveryDocumentCode)) is not null;
            
            var deliveryDocumentCodes = groupedDocument
                .Where(x => !string.IsNullOrWhiteSpace(x.DeliveryDocumentCode))
                .Select(x => x.DeliveryDocumentCode!)
                .ToArray();
            
            var loadingDocumentId = loadingDocuments.FirstOrDefault(x => x.Code == groupedDocument.Key)?.Id ?? Guid.Empty;
            deleteJobs.Add(new DeleteJob(loadingDocumentId, groupedDocument.Key, loadingDocumentShouldBeDeleted, deliveryDocumentCodes));
        }
        
        return deleteJobs.ToArray();
    }

    private record ValidationResult(string? CurrentUserFullName, DeleteJob[] DeleteJobs);
    
    private record DeleteJob(Guid LoadingDocumentId, string LoadingDocumentCode, bool ShouldDeleteLoadingDocument, string[] DeliveryDocumentCodes);
}