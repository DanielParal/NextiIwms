using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;
using Nexticz.Lib.Shared.FileHandling.Models;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Orchestrators;

internal interface IDownloadSignedDocumentsOrchestrator
{
    Task<ErrorOr<FileResult>> OrchestrateAsync(DownloadDocumentJobContract[] downloadDocumentJobs, CancellationToken cancellationToken);
}