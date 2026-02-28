using ErrorOr;

namespace Nexticz.OnPremise.Deployment.Cli.FileBuilders.EnvFileBuild;

internal interface IImageNumberExtractor
{
    ErrorOr<Dictionary<string, string>> GetImageNumbers();
}