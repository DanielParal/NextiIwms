using ErrorOr;

namespace Nexticz.OnPremise.Deployment.Cli.Coordinators;

internal interface IDisplayMenuCoordinator
{
    ErrorOr<MenuOption> GetOption(string environment);
}