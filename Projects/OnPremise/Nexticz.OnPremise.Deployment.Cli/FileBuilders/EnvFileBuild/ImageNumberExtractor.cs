using ErrorOr;
using Nexticz.OnPremise.Deployment.Cli.Configuration;
using Nexticz.OnPremise.Deployment.Cli.Utilities;
using Microsoft.Extensions.Options;

namespace Nexticz.OnPremise.Deployment.Cli.FileBuilders.EnvFileBuild;

internal class ImageNumberExtractor : IImageNumberExtractor
{
    private readonly AzureAcrConfig _azureAcrConfig;
    private readonly FileConfig _fileConfig;

    public ImageNumberExtractor(
        IOptions<AzureAcrConfig> azureAcrConfig,
        IOptions<FileConfig> fileConfig)
    {
        _azureAcrConfig = azureAcrConfig.Value;
        _fileConfig = fileConfig.Value;
    }
    
    public ErrorOr<Dictionary<string, string>> GetImageNumbers()
    {
        var deploymentPath = $"{_fileConfig.RootDirectoryPath}\\{_fileConfig.DeploymentFolder}";
        var versionFileNamePrefix = _fileConfig.VersionFilePrefix;
        
        var result = new Dictionary<string, string>();
        foreach (var repository in _azureAcrConfig.Repositories)
        {
            var searchPattern = $"{versionFileNamePrefix}-{repository.ImageName}-*";
            var fileName = Directory.GetFiles(deploymentPath, searchPattern).FirstOrDefault();

            if (fileName == null)
            {
                return Error.NotFound(
                    ErrorMessages.ImageNumberExtractorNotFoundCode, 
                    ErrorMessages.ImageNumberExtractorNotFoundDescription(versionFileNamePrefix));
            }
            
            var imageNumber = FileNameParser.GetLastPart(fileName);

            if (imageNumber.IsError)
            {
                return Error.Failure(imageNumber.FirstError.Code, imageNumber.FirstError.Description);
            }
            
            result.Add(repository.LocalEnvKey, imageNumber.Value);
        }
        
        return result;
    }
}