namespace Nexticz.OnPremise.Deployment.Cli.Coordinators;

internal interface IUpdateVersionsCoordinator
{
    Task<bool> UpdateVersionsAsync(string environment);
}