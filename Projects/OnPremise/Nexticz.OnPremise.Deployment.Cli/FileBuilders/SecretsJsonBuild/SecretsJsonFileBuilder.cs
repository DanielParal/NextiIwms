using Nexticz.OnPremise.Deployment.Cli.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Nexticz.OnPremise.Deployment.Cli.FileBuilders.SecretsJsonBuild;

internal class SecretsJsonFileBuilder : ISecretsJsonFileBuilder
{
    private readonly FileConfig _fileConfig;
    private readonly ILogger<SecretsJsonFileBuilder> _logger;
    private const string SecretsJsonKey = "IWMS-BE-API-SECRETS-JSON";
    private const string SecretsJsonFileName = "iwms-be-api-secrets.json";
    private string? _secretsJsonValue = string.Empty;
    
    public SecretsJsonFileBuilder(
        IOptions<FileConfig> fileConfig,
        ILogger<SecretsJsonFileBuilder> logger)
    {
        _fileConfig = fileConfig.Value;
        _logger = logger;
    }
    
    public void AddEnvSecretJson(Dictionary<string, string> keyValueSecrets)
    {
        var valueExists = keyValueSecrets.TryGetValue(SecretsJsonKey, out var secretsJsonValue);
        
        if (!valueExists)
            _logger.LogWarning("Secrets json value not found in key value secrets.");
        
        _secretsJsonValue = secretsJsonValue;
    }

    public void BuildSecretsJsonFile()
    {
        try
        {
            var secretsFilePath = Path.Combine(_fileConfig.RootDirectoryPath, SecretsJsonFileName);
            File.WriteAllText(secretsFilePath, _secretsJsonValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create secrets json file: {FileName}. Error: {ErrorMessage}.", SecretsJsonFileName, ex.Message);
            throw;
        }
    }
}