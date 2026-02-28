using Microsoft.AspNetCore.Http;
using Nexticz.Lib.Shared.FileHandling;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.FileHandling;

public interface IDocumentManagerFileHandler : IFileHandler
{
    Task CopyLoadingListFromLoaderFolderToManagerFolderAsync(LoadingDocument loadingDocument, CancellationToken cancellationToken);
    Task<FileResult?> GetDocumentFileFromManagerAsync(string loadingDocumentCode, string documentFileName, CancellationToken cancellationToken);
    Task<FileResult?> GetDocumentFileFromHistoryAsync(string loadingDocumentCode, string? deliveryDocumentCode, CancellationToken cancellationToken);
    Task CopyDocumentFilesFromManagerToHistorySourceFilesFolderAsync(string loadingDocumentCode, string? deliveryDocumentCode, CancellationToken cancellationToken);
    Task RevertDocumentFilesFromHistorySourceFilesToManagerFolderAsync(string loadingDocumentCode,
        string? deliveryDocumentCode, CancellationToken cancellationToken);
    Task SaveSignedDocumentAsync(IFormFile formFile, string loadingDocumentCode, string? deliveryDocumentCode, CancellationToken cancellationToken);
}