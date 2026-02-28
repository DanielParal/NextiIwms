using ErrorOr;

namespace Nexticz.Module.Auth.Application.Accounts.Orchestrators;

public interface ISyncAccountOrchestrator
{
    Task<ErrorOr<Success>> OrchestrateAsync(CancellationToken cancellationToken);
}