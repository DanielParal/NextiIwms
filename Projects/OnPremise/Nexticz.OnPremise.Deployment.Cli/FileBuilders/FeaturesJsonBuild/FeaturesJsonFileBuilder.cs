using Nexticz.OnPremise.Deployment.Cli.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Nexticz.OnPremise.Deployment.Cli.FileBuilders.FeaturesJsonBuild;

internal class FeaturesJsonFileBuilder : IFeaturesJsonFileBuilder
{
    private readonly FileConfig _fileConfig;
    private readonly ILogger<FeaturesJsonFileBuilder> _logger;
    private const string FeaturesJsonKey = "IWMS-BE-API-FEATURES-JSON";
    private const string FeaturesJsonFileName = "iwms-be-api-features.json";
    private string? _featuresJsonValue = string.Empty;
    
    public FeaturesJsonFileBuilder(
        IOptions<FileConfig> fileConfig,
        ILogger<FeaturesJsonFileBuilder> logger)
    {
        _fileConfig = fileConfig.Value;
        _logger = logger;
    }

    public void AddEnvFeatureJson(Dictionary<string, string> keyValueSecrets)
    {
        var valueExists = keyValueSecrets.TryGetValue(FeaturesJsonKey, out var secretsJsonValue);
        
        if (!valueExists)
            _logger.LogWarning("Features json value not found in key value secrets.");
        
        _featuresJsonValue = secretsJsonValue;
    }

    public void BuildFeaturesJsonFile()
    {
        try
        {
            var secretsFilePath = Path.Combine(_fileConfig.RootDirectoryPath, FeaturesJsonFileName);
            File.WriteAllText(secretsFilePath, _featuresJsonValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create features json file: {FileName}. Error: {ErrorMessage}.", FeaturesJsonFileName, ex.Message);
            throw;
        }
    }
}