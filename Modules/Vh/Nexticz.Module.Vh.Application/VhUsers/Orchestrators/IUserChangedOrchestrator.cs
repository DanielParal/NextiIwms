using Nexticz.Module.Auth.Contracts.Accounts;

namespace Nexticz.Module.Vh.Application.VhUsers.Orchestrators;

public interface IUserChangedOrchestrator
{
    Task OrchestrateAsync(UserChangedMessage message, CancellationToken cancellationToken);
}