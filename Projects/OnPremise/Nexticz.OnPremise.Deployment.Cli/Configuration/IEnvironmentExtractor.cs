using ErrorOr;

namespace Nexticz.OnPremise.Deployment.Cli.Configuration;

internal interface IEnvironmentExtractor
{
    ErrorOr<string> GetEnvironment();
}