namespace Nexticz.OnPremise.Deployment.Cli.Coordinators;

internal interface IApplicationCoordinator
{
    Task Orchestrate();
}