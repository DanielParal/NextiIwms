using Nexticz.Module.Auth.Contracts.Accounts;

namespace Nexticz.Module.Sign.Settings.Application.Users.Orchestrators;

internal interface IUserChangedOrchestrator
{
    Task OrchestrateAsync(UserChangedMessage message, CancellationToken cancellationToken);
}