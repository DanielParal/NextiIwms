using ErrorOr;
using Nexticz.OnPremise.Deployment.Cli.Configuration;
using Nexticz.OnPremise.Deployment.Cli.Utilities;
using Microsoft.Extensions.Options;

namespace Nexticz.OnPremise.Deployment.Cli.Configuration;

internal class EnvironmentExtractor : IEnvironmentExtractor
{
    private readonly FileConfig _fileConfig;

    public EnvironmentExtractor(IOptions<FileConfig> fileConfig)
    {
        _fileConfig = fileConfig.Value;
    }
    
    public ErrorOr<string> GetEnvironment()
    {
        var fileNamePrefix = _fileConfig.EnvironmentFilePrefix;
        var deploymentPath = $"{_fileConfig.RootDirectoryPath}\\{_fileConfig.DeploymentFolder}";
        
        var fileName = Directory.GetFiles(deploymentPath, fileNamePrefix + "-*").FirstOrDefault();

        if (fileName == null)
        {
            return Error.NotFound(
                ErrorMessages.EnvironmentExtractorNotFoundCode, 
                ErrorMessages.EnvironmentExtractorNotFoundDescription($"{fileNamePrefix}-{{ENVIRONMENT}}"));
        }
            
        var environment = FileNameParser.GetLastPart(fileName);

        return environment.IsError ? 
            Error.NotFound(
                ErrorMessages.EnvironmentExtractorNotFoundCode, 
                ErrorMessages.EnvironmentExtractorNotFoundDescription($"{fileNamePrefix}-{{ENVIRONMENT}}")) : 
            environment;
    }
}