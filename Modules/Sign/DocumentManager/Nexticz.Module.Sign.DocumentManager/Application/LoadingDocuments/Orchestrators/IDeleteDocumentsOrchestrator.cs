using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Orchestrators;

internal interface IDeleteDocumentsOrchestrator
{
    Task<ErrorOr<Success>> OrchestrateAsync(string deleteReason, DeleteDocumentJobContract[] deleteDocumentJobs, CancellationToken cancellationToken);
}