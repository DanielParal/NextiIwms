using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;
using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates.Queries;
using Nexticz.Module.Sign.SharedKernel.FileHandling;
using Nexticz.Lib.Shared.FileHandling.Assets;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Sign.DocumentManager.Application.FileHandling;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.GetLoadingDocumentByCode;
using Nexticz.Module.Sign.DocumentManager.Application.PdfUtils;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.SignLoadingDocument;

internal class SignLoadingDocumentCommandHandler(
    ISender sender,
    ILogger<SignLoadingDocumentCommandHandler> logger,
    AssetsSettings assetsSettings,
    IDocumentManagerFileHandler fileHandler,
    IDocumentManagerUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<SignLoadingDocumentCommand, ErrorOr<LoadingDocument>>
{
    public async Task<ErrorOr<LoadingDocument>> Handle(SignLoadingDocumentCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateAsync(request, cancellationToken);
        if (validationResult.IsError)
            return validationResult.Errors;
        
        await SignPhysicalFilesAsync(request, validationResult.Value.DeliveryDocumentTemplate, validationResult.Value.LoadingDocumentTemplate, cancellationToken);
        
        return SaveEvents(validationResult.Value.LoadingDocument, request, validationResult.Value.DepositorName);
    }

    private async Task<ErrorOr<ValidationResult>> ValidateAsync(SignLoadingDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var loadingDocument = await sender.Send(new GetLoadingDocumentByCodeQuery(request.LoadingDocumentCode),
            cancellationToken);
        if (loadingDocument.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - object {ObjectName} does not exist. Cannot sign loading document. Current user: {UserName}, loading document code: {LoadingDocumentCode}.",
                nameof(LoadingDocument), request.UserName, request.LoadingDocumentCode);
            return LoadingDocumentErrors.ValidationLoadingDocumentDoesNotExist;
        }

        if (request.ShouldAlsoSignLoadingDocument)
        {
            var canLoadingDocumentBeSigned = loadingDocument.Value.CanLoadingDocumentBeSigned();
            if (canLoadingDocumentBeSigned.IsError)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - cannot sign loading document. ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}. " +
                    "LoadingDocumentCode: {LoadingDocumentCode}, current user: {UserName}.",
                    canLoadingDocumentBeSigned.FirstError.Code, canLoadingDocumentBeSigned.FirstError.Description,
                    loadingDocument.Value.Code, request.UserName);
                return canLoadingDocumentBeSigned.Errors;
            }
        }
        
        var canDeliveryDocumentsBeSigned = loadingDocument.Value.CanDeliveryDocumentsBeSigned(request.DeliveryDocumentCodes);
        if (canDeliveryDocumentsBeSigned.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - cannot sign all delivery documents for loading document. ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}. " +
                "LoadingDocumentCode: {LoadingDocumentCode}, current user: {UserName}.",
                canDeliveryDocumentsBeSigned.FirstError.Code, canDeliveryDocumentsBeSigned.FirstError.Description,
                loadingDocument.Value.Code, request.UserName);
            return canDeliveryDocumentsBeSigned.Errors;
        }
        
        var templates = await GetDocumentTemplatesAsync(loadingDocument.Value.DepositorCode, cancellationToken);
        if (templates.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - cannot sign documents because document templates don't exist. ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}. " +
                "LoadingDocumentCode: {LoadingDocumentCode}, current user: {UserName}.",
                templates.FirstError.Code, templates.FirstError.Description,
                loadingDocument.Value.Code, request.UserName);
            return LoadingDocumentErrors.ValidationDocumentTemplatesDontExist;
        }
        
        var depositorName = await GetDepositorNameAsync(loadingDocument.Value.DepositorCode, cancellationToken);

        return new ValidationResult(loadingDocument.Value, templates.Value.DeliveryDocumentTemplate, templates.Value.LoadingDocumentTemplate, depositorName);
    }

    private async Task<ErrorOr<(DocumentTemplateResponse DeliveryDocumentTemplate, DocumentTemplateResponse LoadingDocumentTemplate)>> GetDocumentTemplatesAsync(string depositorCode,
        CancellationToken cancellationToken)
    {
        var depositor = await sender.Send(new GetDepositorResponseByCodeQuery(depositorCode), cancellationToken);
        if (depositor.IsError)
            return depositor.Errors;
        
        var documentTemplate = await sender.Send(new GetDocumentTemplateResponseByCodeQuery(depositor.Value.DeliveryTemplateCode), cancellationToken);
        if (documentTemplate.IsError)
            return documentTemplate.Errors;
        
        var loadingTemplate = await sender.Send(new GetDocumentTemplateResponseByCodeQuery(depositor.Value.LoadingTemplateCode), cancellationToken);
        if (loadingTemplate.IsError)
            return loadingTemplate.Errors;
        
        return (documentTemplate.Value, loadingTemplate.Value);
    }

    private async Task SignPhysicalFilesAsync(SignLoadingDocumentCommand request, DocumentTemplateResponse deliveryDocumentTemplate, 
        DocumentTemplateResponse loadingDocumentTemplate, CancellationToken cancellationToken)
    {
        var loadingDocumentCodeToSign = request.LoadingDocumentCode;
        
        if (request.ShouldAlsoSignLoadingDocument)
        {
            var inputPath = Path.Combine(DirectoryNamesProvider.GetBaseManagerPath(assetsSettings), loadingDocumentCodeToSign,
                $"{loadingDocumentCodeToSign}.pdf");

            var outputPath = DirectoryNamesProvider.GetHistoryLoadingDocumentFilePath(assetsSettings, loadingDocumentCodeToSign);
            
            var signedAtFormatted = clock.ConvertUtcToTenantDateTime(request.SignedAt).ToString("dd.MM.yyyy");
            LoadingDocumentPdfSigner.SignPdf(loadingDocumentTemplate,
                inputPath, outputPath, request.DriverSignatureFile.ContentBytes, request.UserWhoSentDocumentsSignatureFile.ContentBytes, 
                request.UserFullName, signedAtFormatted, request.SignedAt.LocalDateTime.ToString("HH:mm"), 
                request.DriverName, request.LicensePlate, signedAtFormatted, request.SignedAt.LocalDateTime.ToString("HH:mm"));
            
            await fileHandler.CopyDocumentFilesFromManagerToHistorySourceFilesFolderAsync(loadingDocumentCodeToSign, null, cancellationToken);
        }
        
        foreach (var deliveryDocumentCodeToSign in request.DeliveryDocumentCodes)
        {
            var deliveryDocumentInputPath = Path.Combine(DirectoryNamesProvider.GetBaseManagerPath(assetsSettings), loadingDocumentCodeToSign,
                $"{deliveryDocumentCodeToSign}.pdf");
            var deliveryDocumentOutputPath = DirectoryNamesProvider.GetHistoryDeliveryDocumentFilePath(assetsSettings, loadingDocumentCodeToSign, deliveryDocumentCodeToSign);
            
            DeliveryDocumentPdfSigner.SignPdf(deliveryDocumentTemplate, deliveryDocumentInputPath, deliveryDocumentOutputPath, request.DriverSignatureFile.ContentBytes, 
                request.UserWhoSentDocumentsSignatureFile.ContentBytes, request.UserFullName, request.DriverName, clock.TenantNowOffset.ToString("dd.MM.yyyy"));
            
            await fileHandler.CopyDocumentFilesFromManagerToHistorySourceFilesFolderAsync(loadingDocumentCodeToSign, deliveryDocumentCodeToSign, cancellationToken);
        }
    }

    private LoadingDocument SaveEvents(LoadingDocument loadingDocument, SignLoadingDocumentCommand request, string? depositorName)
    {
        loadingDocument.SignDocument(request.ShouldAlsoSignLoadingDocument, request.DeliveryDocumentCodes, 
            request.SignedAt, request.UserFullName, request.UserName, request.DriverName, request.LicensePlate, depositorName);
        
        var loadingDocumentSignedEvent = new LoadingDocumentSignedEvent(
            loadingDocument.Id, loadingDocument.Code, request.ShouldAlsoSignLoadingDocument, 
            request.DeliveryDocumentCodes, request.SignedAt, request.UserName, request.UserFullName,
            request.DriverName, request.LicensePlate, depositorName);
        unitOfWork.AppendEvent(loadingDocument.Id, loadingDocumentSignedEvent);
        
        logger.LogInformation("SIGN - DocumentManager - loading document signed. LoadingDocumentCode: {LoadingDocumentCode}, SignedAt: {SignedAt}, DeliveryDocumentCodes: {DeliveryDocumentCodes}, LicensePlate: {LicensePlate}.", 
            loadingDocument.Code, request.SignedAt, request.DeliveryDocumentCodes, request.LicensePlate);

        if (loadingDocument.IsLoadingDocumentFullyFinishedWithAllDeliveryDocuments())
        {
            var loadingDocumentFullyFinishedEvent = new LoadingDocumentWithAllDeliveryDocumentsFinishedEvent(
                loadingDocument.Id, loadingDocument.Code);
            unitOfWork.AppendEvent(loadingDocument.Id, loadingDocumentFullyFinishedEvent);
            
            logger.LogInformation("SIGN - DocumentManager - loading document is fully finished with all delivery documents after signing documents. LoadingDocumentId: {LoadingDocumentId}, LoadingDocumentCode: {LoadingDocumentCode}.", 
                loadingDocument.Id, loadingDocument.Code);
        }
        
        return loadingDocument;
    }
    
    private async Task<string?> GetDepositorNameAsync(string depositorCode, CancellationToken cancellationToken)
    {
        var depositorResponse = await sender.Send(new GetDepositorResponseByCodeQuery(depositorCode), cancellationToken);
        return depositorResponse.IsError ? null : depositorResponse.Value.Name;
    }
    
    private record ValidationResult(LoadingDocument LoadingDocument, DocumentTemplateResponse DeliveryDocumentTemplate, DocumentTemplateResponse LoadingDocumentTemplate, string? DepositorName);
}