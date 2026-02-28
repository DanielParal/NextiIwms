using ErrorOr;

namespace Nexticz.Module.Auth.Application.Users.Orchestrators;

public interface ISyncUserOrchestrator
{
    Task<ErrorOr<Success>> OrchestrateAsync(CancellationToken cancellationToken);
}