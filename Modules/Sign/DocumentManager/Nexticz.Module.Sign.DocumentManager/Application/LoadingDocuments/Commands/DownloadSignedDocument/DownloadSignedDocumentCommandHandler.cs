using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Module.Sign.DocumentManager.Application.FileHandling;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.DownloadSignedDocument;

internal class DownloadSignedDocumentCommandHandler(
    ILogger<DownloadSignedDocumentCommandHandler> logger,
    IDocumentManagerUnitOfWork unitOfWork,
    IDocumentManagerFileHandler fileHandler) : IRequestHandler<DownloadSignedDocumentCommand, ErrorOr<FileResult>>
{
    public async Task<ErrorOr<FileResult>> Handle(DownloadSignedDocumentCommand request, CancellationToken cancellationToken)
    {
        var fileResult = 
            await GetFileResultAsync(
                request.LoadingDocumentCode,
                request.DeliveryDocumentCode, 
                cancellationToken);
        
        if (fileResult.IsError)
            return fileResult;

        var documentDownloadedEvent = 
            new DocumentDownloadedEvent(
                request.LoadingDocumentId,
                request.LoadingDocumentCode,
                request.DeliveryDocumentCode,
                request.DownloadDate,
                request.CurrentUserName
            );
        
        unitOfWork.AppendEvent(request.LoadingDocumentId, documentDownloadedEvent);
        
        logger.LogInformation("Sign - Manager - Downloaded signed document. LoadingDocumentId: {LoadingDocumentId}, LoadingDocumentCode: {LoadingDocumentCode}, DeliveryDocumentCode: {DeliveryDocumentCode}", 
            request.LoadingDocumentId, request.LoadingDocumentCode, request.DeliveryDocumentCode);
        
        return fileResult.Value;
    }

    private async Task<ErrorOr<FileResult>> GetFileResultAsync(
        string loadingDocumentCode,
        string? deliveryDocumentCode,
        CancellationToken cancellationToken)
    {
        var fileResult = await fileHandler.GetDocumentFileFromHistoryAsync(loadingDocumentCode, deliveryDocumentCode, cancellationToken);

        if (fileResult is null)
        {
            logger.LogWarning("Sign - Document manager - Cannot download document because file not found. LoadingDocumentCode: {LoadingDocumentCode}, DeliveryDocumentCode: {DeliveryDocumentCode}.",
                loadingDocumentCode, deliveryDocumentCode);
            return LoadingDocumentErrors.ValidationFileNotFound;
        }
        
        return fileResult;
    }
}