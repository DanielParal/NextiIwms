namespace Nexticz.OnPremise.Deployment.Cli.Coordinators;

internal interface IDeploymentCoordinator
{
    Task<bool> DeployAsync(string environment);
}