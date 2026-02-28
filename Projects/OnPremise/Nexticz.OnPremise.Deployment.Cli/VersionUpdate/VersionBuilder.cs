using Nexticz.OnPremise.Deployment.Cli.Configuration;
using Microsoft.Extensions.Options;
using ErrorOr;
using Nexticz.OnPremise.Deployment.Cli.Utilities;

namespace Nexticz.OnPremise.Deployment.Cli.VersionUpdate;

internal class VersionBuilder : IVersionBuilder
{
    private readonly FileConfig _fileConfig;

    public VersionBuilder(IOptions<FileConfig> fileConfig)
    {
        _fileConfig = fileConfig.Value;
    }
    
    public ErrorOr<int> CreateVersionFiles(Dictionary<string, string> imageVersions)
    {
        var deploymentPath = $"{_fileConfig.RootDirectoryPath}\\{_fileConfig.DeploymentFolder}";
        var filePattern = _fileConfig.VersionFilePrefix;
        var numberOfFilesCreated = 0;

        try
        {
            foreach (var imageVersion in imageVersions)
            {
                var filesToDelete = Directory.GetFiles(deploymentPath, $"{filePattern}-{imageVersion.Key}-*");

                foreach (string file in filesToDelete)
                {
                    File.Delete(file);
                }

                File.Create($"{deploymentPath}\\{filePattern}-{imageVersion.Key}-{imageVersion.Value}").Dispose();
                numberOfFilesCreated++;
            }

            return numberOfFilesCreated;
        }
        catch (Exception ex)
        {
            return Error.Failure(ErrorMessages.VersionBuilderCreateVersionFileCode, ErrorMessages.VersionBuilderCreateVersionFileDescription(ex.Message));
        }
    }
}