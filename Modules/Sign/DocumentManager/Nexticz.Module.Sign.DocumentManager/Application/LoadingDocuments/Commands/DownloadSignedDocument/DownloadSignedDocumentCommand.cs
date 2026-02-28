using ErrorOr;
using Nexticz.Lib.Shared.FileHandling.Models;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.DownloadSignedDocument;

internal record DownloadSignedDocumentCommand(
    Guid LoadingDocumentId,
    string LoadingDocumentCode, 
    string? DeliveryDocumentCode, 
    DateTimeOffset DownloadDate,
    string CurrentUserName) 
    : IDocumentManagerCommand<ErrorOr<FileResult>>;