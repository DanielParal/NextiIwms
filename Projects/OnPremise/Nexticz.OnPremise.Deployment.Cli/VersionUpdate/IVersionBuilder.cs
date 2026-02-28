using ErrorOr;

namespace Nexticz.OnPremise.Deployment.Cli.VersionUpdate;

internal interface IVersionBuilder
{
    ErrorOr<int> CreateVersionFiles(Dictionary<string, string> imageVersions);
}