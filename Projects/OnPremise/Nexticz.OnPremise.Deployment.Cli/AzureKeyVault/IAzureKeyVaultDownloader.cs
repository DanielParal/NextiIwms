using ErrorOr;

namespace Nexticz.OnPremise.Deployment.Cli.AzureKeyVault;

internal interface IAzureKeyVaultDownloader
{
    Task<ErrorOr<Dictionary<string, string>>> GetSecretsAsync();
}