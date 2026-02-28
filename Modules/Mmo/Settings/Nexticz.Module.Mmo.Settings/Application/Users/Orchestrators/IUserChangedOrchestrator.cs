using Nexticz.Module.Auth.Contracts.Accounts;

namespace Nexticz.Module.Mmo.Settings.Application.Users.Orchestrators;

internal interface IUserChangedOrchestrator
{
    Task OrchestrateAsync(UserChangedMessage message, CancellationToken cancellationToken);
}