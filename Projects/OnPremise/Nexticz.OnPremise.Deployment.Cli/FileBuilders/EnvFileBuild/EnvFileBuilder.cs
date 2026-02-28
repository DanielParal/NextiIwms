using Nexticz.OnPremise.Deployment.Cli.Configuration;
using Microsoft.Extensions.Options;

namespace Nexticz.OnPremise.Deployment.Cli.FileBuilders.EnvFileBuild;

internal class EnvFileBuilder : IEnvFileBuilder
{
    private readonly FileConfig _fileConfig;
    private readonly EnvConfig _envConfig;
    private static Dictionary<string, string> _envSecrets = new();

    public EnvFileBuilder(
        IOptions<FileConfig> fileConfig,
        IOptions<EnvConfig> envConfig)
    {
        _fileConfig = fileConfig.Value;
        _envConfig = envConfig.Value;
    }

    public void BuildEnvFile()
    {
        var envFilePath = $"{_fileConfig.RootDirectoryPath}\\.env";
        AddIwmsFeVersions(_envConfig.IwmsFeVersions);
        CreateEnvFile(envFilePath);
    }
    
    public void AddEnvSecrets(Dictionary<string, string> envSecrets)
    {
        foreach (var envSecret in envSecrets)
        {
            _envSecrets.Add(SanitizeKey(envSecret.Key), envSecret.Value);
        }
    }
    
    private static void CreateEnvFile(string filePath)
    {
        using var writer = new StreamWriter(filePath);
        foreach (var kvp in _envSecrets)
        {
            writer.WriteLine($"{kvp.Key}={kvp.Value}");
        }
    }
    
    private static void AddIwmsFeVersions(EnvConfig.IwmsFeVersion[] versions)
    {
        foreach (var iwmsFeVersion in versions)
        {
            if (!_envSecrets.TryGetValue(iwmsFeVersion.ImageNumberLookupKey, out var imageNumber))
            {
                continue;
            }
            
            _envSecrets.Add(iwmsFeVersion.VersionKey, $"({iwmsFeVersion.MajorVersion}{imageNumber} - {DateTime.Now:yyyy.MM.dd})");
        }
        
    }

    private static string SanitizeKey(string key)
    {
        // In azure key vault we can store keys only using hyphens. In docker compose we use underscore
        return key.Replace("-", "_");
    }
}