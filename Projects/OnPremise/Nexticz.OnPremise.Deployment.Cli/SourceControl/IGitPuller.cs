using ErrorOr;

namespace Nexticz.OnPremise.Deployment.Cli.SourceControl;

internal interface IGitPuller
{
    Task<ErrorOr<string>> DownloadLatestDockerComposeProject();
}