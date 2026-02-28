using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;
using Nexticz.Module.Sign.DocumentManager.Application.FileHandling;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.RevertLoadingDocumentsFiles;

internal class RevertLoadingDocumentsFilesCommandHandler(
    IDocumentManagerFileHandler fileHandler) : IRequestHandler<RevertLoadingDocumentsFilesCommand, Success>
{
    public async Task<Success> Handle(RevertLoadingDocumentsFilesCommand request, CancellationToken cancellationToken)
    {
        if (request.RevertLoadingDocumentJobs.Length == 0)
            return Result.Success;

        foreach (var revertLoadingDocumentJob in request.RevertLoadingDocumentJobs)
        {
            await RevertFilesForLoadingDocumentAsync(revertLoadingDocumentJob, cancellationToken);
        }
        
        return Result.Success;
    }
    
    private async Task<Success> RevertFilesForLoadingDocumentAsync(RevertLoadingDocumentFileJob revertLoadingDocumentFile, CancellationToken cancellationToken)
    {
        if (revertLoadingDocumentFile.ShouldLoadingDocumentBeReverted)
        {
            await fileHandler.RevertDocumentFilesFromHistorySourceFilesToManagerFolderAsync(revertLoadingDocumentFile.LoadingDocumentCode, null, cancellationToken);
        }
        
        foreach (var deliveryDocumentCode in revertLoadingDocumentFile.DeliveryDocumentCodes)
        {
            await fileHandler.RevertDocumentFilesFromHistorySourceFilesToManagerFolderAsync(revertLoadingDocumentFile.LoadingDocumentCode, deliveryDocumentCode, cancellationToken);
        }
        
        return Result.Success;
    }
}