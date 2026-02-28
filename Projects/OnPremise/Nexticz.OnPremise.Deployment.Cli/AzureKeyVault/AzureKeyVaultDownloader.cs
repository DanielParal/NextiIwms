using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using ErrorOr;
using Nexticz.OnPremise.Deployment.Cli.Configuration;
using Nexticz.OnPremise.Deployment.Cli.Logging;
using Nexticz.OnPremise.Deployment.Cli.Utilities;
using Microsoft.Extensions.Options;
using Exception = System.Exception;

namespace Nexticz.OnPremise.Deployment.Cli.AzureKeyVault;

internal class AzureKeyVaultDownloader : IAzureKeyVaultDownloader
{
    private readonly AzureKvConfig _azureKvConfig;
    private readonly IEnhancedLogger<AzureKeyVaultDownloader> _logger;

    public AzureKeyVaultDownloader(
        IOptions<AzureKvConfig> azureKvConfig,
        IEnhancedLogger<AzureKeyVaultDownloader> logger)
    {
        _azureKvConfig = azureKvConfig.Value;
        _logger = logger;
    }
    
    public async Task<ErrorOr<Dictionary<string, string>>> GetSecretsAsync()
    {
        try
        {
            var client = new SecretClient(new Uri(_azureKvConfig.KeyVaultUrl), new DefaultAzureCredential());
            
            var kvNames = await ListSecretNamesAsync(client);

            var secrets = new Dictionary<string, string>();
            foreach (var kvName in kvNames)
            {
                var kvValue = await GetSecretValueAsync(client, kvName);
                secrets.Add(kvName, kvValue);
            }
            
            return secrets;
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception calling azure keyvault for secrets. Exception message - {ex.Message}", ex);
            return Error.Failure(
                ErrorMessages.AzureKeyVaultDownloaderFailureCode,
                ErrorMessages.AzureKeyVaultDownloaderFailureDescription(ex.Message));
        }
    }
    
    private static async Task<string> GetSecretValueAsync(SecretClient client, string secretName)
    {
        KeyVaultSecret secret = await client.GetSecretAsync(secretName);
        return secret.Value;
    }
    
    private static async Task<List<string>> ListSecretNamesAsync(SecretClient client)
    {
        var result = new List<string>();
        await foreach (var secretProperties in client.GetPropertiesOfSecretsAsync())
        {
            result.Add(secretProperties.Name);;
        }
        
        return result;
    }
}