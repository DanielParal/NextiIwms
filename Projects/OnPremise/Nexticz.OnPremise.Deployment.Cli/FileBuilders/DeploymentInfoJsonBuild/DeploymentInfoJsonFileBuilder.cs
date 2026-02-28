using System.Text.Json;
using Nexticz.OnPremise.Deployment.Cli.Configuration;
using Nexticz.OnPremise.Deployment.Cli.FileBuilders.DeploymentInfoJsonBuild.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DeploymentInfoJsonContext = Nexticz.OnPremise.Deployment.Cli.FileBuilders.DeploymentInfoJsonBuild.Models.DeploymentInfoJsonContext;

namespace Nexticz.OnPremise.Deployment.Cli.FileBuilders.DeploymentInfoJsonBuild;

internal class DeploymentInfoJsonFileBuilder(
    IOptions<FileConfig> fileConfig,
    ILogger<DeploymentInfoJsonFileBuilder> logger)
    : IDeploymentInfoJsonFileBuilder
{
    private readonly FileConfig _fileConfig = fileConfig.Value;
    private const string DeploymentInfoJsonFileName = "iwms-be-api-deployment-info.json";

    public void BuildDeploymentInfoJsonFile()
    {
        try
        {
            var deploymentInfo = new DeploymentInfoContainer
            {
                DeploymentInfoSettings = new DeploymentInfoSettings
                {
                    LastDeploymentTimestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                }
            };
            
            var jsonContent = JsonSerializer.Serialize(deploymentInfo, DeploymentInfoJsonContext.Default.DeploymentInfoContainer);
            
            var deploymentInfoFilePath = Path.Combine(_fileConfig.RootDirectoryPath, DeploymentInfoJsonFileName);
            File.WriteAllText(deploymentInfoFilePath, jsonContent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create deployment info json file: {FileName}. Error: {ErrorMessage}.", DeploymentInfoJsonFileName, ex.Message);
            throw;
        }
    }
}