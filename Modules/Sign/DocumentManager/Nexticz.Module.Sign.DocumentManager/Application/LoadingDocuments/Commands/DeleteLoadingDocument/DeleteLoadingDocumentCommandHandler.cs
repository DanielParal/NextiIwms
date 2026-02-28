using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Sign.SharedKernel.FileHandling;
using Nexticz.Lib.Shared.FileHandling.Assets;
using Nexticz.Module.Sign.DocumentManager.Application.FileHandling;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.GetLoadingDocumentByCode;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.DeleteLoadingDocument;

internal class DeleteLoadingDocumentCommandHandler(
    ISender sender,
    ILogger<DeleteLoadingDocumentCommandHandler> logger,
    AssetsSettings assetsSettings,
    IDocumentManagerFileHandler fileHandler,
    IDocumentManagerUnitOfWork unitOfWork) : IRequestHandler<DeleteLoadingDocumentCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteLoadingDocumentCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateAsync(request, cancellationToken);
        if (validationResult.IsError)
            return validationResult.Errors;
        
        await CopyPhysicalFilesToHistoryAsync(request, cancellationToken);
        
        return SaveEvents(request, validationResult.Value.LoadingDocument, validationResult.Value.DepositorName);
    }

    private async Task<ErrorOr<ValidationResult>> ValidateAsync(DeleteLoadingDocumentCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.DeleteReason))
        {
            logger.LogWarning(
                "SIGN - DocumentManager - object {ObjectName} - there is no reason for deletion. Cannot delete loading document. " +
                "Current user: {UserName}, loading document code: {LoadingDocumentCode}, loading document id: {LoadingDocumentId}.",
                nameof(LoadingDocument), request.DeletedByUserName, request.LoadingDocumentCode, request.LoadingDocumentId);
            return LoadingDocumentErrors.ValidationNoReasonForDeletion;
        }
        
        var loadingDocument = await sender.Send(new GetLoadingDocumentByCodeQuery(request.LoadingDocumentCode),
            cancellationToken);
        if (loadingDocument.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - object {ObjectName} does not exist. Cannot delete loading document. " +
                "Current user: {UserName}, loading document code: {LoadingDocumentCode}, loading document id: {LoadingDocumentId}.",
                nameof(LoadingDocument), request.DeletedByUserName, request.LoadingDocumentCode, request.LoadingDocumentId);
            return LoadingDocumentErrors.ValidationLoadingDocumentDoesNotExist;
        }

        var canDocumentsBeDeleted =
            loadingDocument.Value.CanLoadingDocumentWithDeliveryDocumentsBeDeleted(
                request.ShouldLoadingDocumentBeAlsoDeleted, request.DeliveryDocumentCodes);
        if (canDocumentsBeDeleted.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - object {ObjectName} cannot be deleted. Current user: {UserName}, " +
                "loading document code: {LoadingDocumentCode}, delivery document codes: {DeliveryDocumentCodes}. " +
                "Domain error code: {DomainErrorCode}, Domain error message: {DomainErrorMessage}.",
                nameof(LoadingDocument), request.DeletedByUserName, request.LoadingDocumentCode,
                request.DeliveryDocumentCodes, canDocumentsBeDeleted.FirstError.Code, canDocumentsBeDeleted.FirstError.Description);
            return canDocumentsBeDeleted.Errors;
        }
        
        var depositorName = await GetDepositorNameAsync(loadingDocument.Value.DepositorCode, cancellationToken);

        return new ValidationResult(loadingDocument.Value, depositorName);
    }

    private async Task CopyPhysicalFilesToHistoryAsync(DeleteLoadingDocumentCommand request, CancellationToken cancellationToken)
    {
        var loadingDocumentCodeToSign = request.LoadingDocumentCode;

        if (request.ShouldLoadingDocumentBeAlsoDeleted)
        {
            var fileName = $"{loadingDocumentCodeToSign}.pdf";
            var inputFilePath = Path.Combine(DirectoryNamesProvider.GetBaseManagerPath(assetsSettings), loadingDocumentCodeToSign, fileName);
            var outputFilePath = DirectoryNamesProvider.GetHistoryLoadingDocumentFilePath(assetsSettings, loadingDocumentCodeToSign);
            
            var inputFolderPath = Path.GetDirectoryName(inputFilePath)!;
            var outputFolderPath = Path.GetDirectoryName(outputFilePath)!;

            await fileHandler.CopyFileFromSourceToDestinationAsync(fileName, fileName, inputFolderPath, outputFolderPath, false, false, cancellationToken);
            await fileHandler.CopyDocumentFilesFromManagerToHistorySourceFilesFolderAsync(loadingDocumentCodeToSign, null, cancellationToken);
        }
        
        foreach (var deliveryDocumentCodeToSign in request.DeliveryDocumentCodes)
        {
            var fileName = $"{deliveryDocumentCodeToSign}.pdf";
            var deliveryDocumentInputFilePath = Path.Combine(DirectoryNamesProvider.GetBaseManagerPath(assetsSettings), loadingDocumentCodeToSign, fileName);
            var deliveryDocumentOutputFilePath = DirectoryNamesProvider.GetHistoryDeliveryDocumentFilePath(assetsSettings, loadingDocumentCodeToSign, deliveryDocumentCodeToSign);
            
            var inputFolderPath = Path.GetDirectoryName(deliveryDocumentInputFilePath)!;
            var outputFolderPath = Path.GetDirectoryName(deliveryDocumentOutputFilePath)!;

            await fileHandler.CopyFileFromSourceToDestinationAsync(fileName, fileName, inputFolderPath, outputFolderPath, false, false, cancellationToken);
            await fileHandler.CopyDocumentFilesFromManagerToHistorySourceFilesFolderAsync(loadingDocumentCodeToSign, deliveryDocumentCodeToSign, cancellationToken);
        }
    }

    private Success SaveEvents(DeleteLoadingDocumentCommand request, LoadingDocument loadingDocument, string? depositorName)
    {
        loadingDocument.DeleteDocument(
            request.ShouldLoadingDocumentBeAlsoDeleted, request.DeliveryDocumentCodes, 
            request.DeletedAt,request.DeleteReason, request.DeletedByUserName, request.DeletedByUserFullName, depositorName);
        
        var loadingDocumentDeletedEvent = new LoadingDocumentDeletedEvent(
            loadingDocument.Id, loadingDocument.Code, request.ShouldLoadingDocumentBeAlsoDeleted, 
            request.DeliveryDocumentCodes, request.DeletedAt, request.DeletedByUserName, 
            request.DeletedByUserFullName, depositorName, request.DeleteReason);
        
        unitOfWork.AppendEvent(loadingDocument.Id, loadingDocumentDeletedEvent);
        
        logger.LogInformation("SIGN - DocumentManager - loading document deleted. LoadingDocumentCode: {LoadingDocumentCode}, DeletedAt: {DeletedAt}, DeliveryDocumentCodes: {DeliveryDocumentCodes}, Reason: {DeleteReason}", 
            loadingDocument.Code, request.DeletedAt, request.DeliveryDocumentCodes, request.DeleteReason);
        
        if (loadingDocument.IsLoadingDocumentFullyFinishedWithAllDeliveryDocuments())
        {
            var loadingDocumentFullyFinishedEvent = new LoadingDocumentWithAllDeliveryDocumentsFinishedEvent(
                loadingDocument.Id, loadingDocument.Code);
            unitOfWork.AppendEvent(loadingDocument.Id, loadingDocumentFullyFinishedEvent);
            
            logger.LogInformation("SIGN - DocumentManager - loading document is fully finished with all delivery documents after signing documents. LoadingDocumentId: {LoadingDocumentId}, LoadingDocumentCode: {LoadingDocumentCode}.", 
                loadingDocument.Id, loadingDocument.Code);
        }
        
        return Result.Success;
    }
    
    private async Task<string?> GetDepositorNameAsync(string depositorCode, CancellationToken cancellationToken)
    {
        var depositorResponse = await sender.Send(new GetDepositorResponseByCodeQuery(depositorCode), cancellationToken);
        return depositorResponse.IsError ? null : depositorResponse.Value.Name;
    }
    
    private record ValidationResult(LoadingDocument LoadingDocument, string? DepositorName);
}