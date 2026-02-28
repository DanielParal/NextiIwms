using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Contracts.Printings;

namespace Nexticz.Module.Sign.DocumentManager.Application.PrintingHandling;

internal interface IDocumentsPrintingOrchestrator
{
    Task<ErrorOr<Success>> OrchestrateAsync(DocumentsPrintingRequested message, bool isLastAttempt, CancellationToken cancellationToken);
}