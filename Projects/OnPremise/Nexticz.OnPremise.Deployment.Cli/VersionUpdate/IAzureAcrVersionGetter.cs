using ErrorOr;

namespace Nexticz.OnPremise.Deployment.Cli.VersionUpdate;

internal interface IAzureAcrVersionGetter
{
    Task<ErrorOr<Dictionary<string, string>>> GetLatestImageVersions();
}