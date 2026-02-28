using ErrorOr;

namespace Nexticz.OnPremise.Deployment.Cli.Docker;

internal interface IDockerComposeRunner
{
    Task<ErrorOr<string>> RunDockerComposeUp();
}