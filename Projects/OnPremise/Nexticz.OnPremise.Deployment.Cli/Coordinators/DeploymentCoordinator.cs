using Nexticz.OnPremise.Deployment.Cli.AzureKeyVault;
using Nexticz.OnPremise.Deployment.Cli.AzureLogin;
using Nexticz.OnPremise.Deployment.Cli.Docker;
using Nexticz.OnPremise.Deployment.Cli.FileBuilders.DeploymentInfoJsonBuild;
using Nexticz.OnPremise.Deployment.Cli.FileBuilders.EnvFileBuild;
using Nexticz.OnPremise.Deployment.Cli.FileBuilders.FeaturesJsonBuild;
using Nexticz.OnPremise.Deployment.Cli.FileBuilders.SecretsJsonBuild;
using Nexticz.OnPremise.Deployment.Cli.Logging;

namespace Nexticz.OnPremise.Deployment.Cli.Coordinators;

internal class DeploymentCoordinator(
    IAzureLoginRefresher azLoginRefresher,
    IImageNumberExtractor imageNumberExtractor,
    IAzureKeyVaultDownloader azureKeyVaultDownloader,
    IEnvFileBuilder envFileBuilder,
    ISecretsJsonFileBuilder secretsJsonFileBuilder,
    IDockerComposeRunner dockerComposeRunner,
    IEnhancedLogger<DeploymentCoordinator> logger,
    IDeploymentInfoJsonFileBuilder deploymentInfoJsonFileBuilder,
    IFeaturesJsonFileBuilder featuresJsonFileBuilder)
    : IDeploymentCoordinator
{
    public async Task<bool> DeployAsync(string environment)
    {
        // #1 azure login refresher
        logger.LogInformation($"{environment.ToUpper()}: updating token for azure.", true);
        var loginResult = await azLoginRefresher.Login();

        if (loginResult.IsError)
        {
            logger.LogError($"{environment.ToUpper()}: error refreshing token for Azure. Error: {loginResult.FirstError.Description}", true);
            return false;
        }
        
        logger.LogInformation($"{environment.ToUpper()}: azure refreshed token successfully. Pulling latest changes from git.", true);
        
        // #2 get latest image numbers
        var imageNumbersResult = imageNumberExtractor.GetImageNumbers();
        
        if (imageNumbersResult.IsError)
        {
            logger.LogError($"{environment.ToUpper()}: error getting latest image numbers. Error: {imageNumbersResult.FirstError.Description}", true);
            return false;
        }
        
        logger.LogInformation($"{environment.ToUpper()}: get latest image numbers. Getting secrets from Azure key vault.", true);
        
        // #3 get secrets from azure key vault
        var keyVaultSecretsResult = await azureKeyVaultDownloader.GetSecretsAsync();
        
        if (keyVaultSecretsResult.IsError)
        {
            logger.LogError($"{environment.ToUpper()}: error getting secrets from Azure key vault. Error: {keyVaultSecretsResult.FirstError.Description}", true);
            return false;
        }
        
        logger.LogInformation($"{environment.ToUpper()}: get secrets from Azure key vault. Number of secrets downloaded: {keyVaultSecretsResult.Value.Count}. Building .env file.", true);
        
        // #4 build .env file
        envFileBuilder.AddEnvSecrets(imageNumbersResult.Value);
        envFileBuilder.AddEnvSecrets(keyVaultSecretsResult.Value);
        envFileBuilder.BuildEnvFile();
        
        logger.LogInformation($"{environment.ToUpper()}: .env file is built. Building iwms-be-api-secrets.json file.", true);
        
        // #5 build iwms-be-api-secrets.json file
        secretsJsonFileBuilder.AddEnvSecretJson(keyVaultSecretsResult.Value);
        secretsJsonFileBuilder.BuildSecretsJsonFile();
        
        logger.LogInformation($"{environment.ToUpper()}: iwms-be-api-secrets.json file is built. Building iwms-be-api-deployment-info.json file.", true);
        
        // #6 build iwms-be-api-deployment-info.json file
        deploymentInfoJsonFileBuilder.BuildDeploymentInfoJsonFile();
        
        logger.LogInformation($"{environment.ToUpper()}: iwms-be-api-deployment-info.json file is built. Building iwms-be-api-features.json file.", true);
        
        // #7 build iwms-be-api-features.json file
        featuresJsonFileBuilder.AddEnvFeatureJson(keyVaultSecretsResult.Value);
        featuresJsonFileBuilder.BuildFeaturesJsonFile();
        
        logger.LogInformation($"{environment.ToUpper()}: iwms-be-api-features.json file is built. Running docker compose and redeploying application.", true);
        
        // #8 run docker compose up
        var dockerRunnerResult = await dockerComposeRunner.RunDockerComposeUp();
        
        if (dockerRunnerResult.IsError)
        {
            logger.LogError($"{environment.ToUpper()}: error start docker compose up. Error: {dockerRunnerResult.FirstError.Description}", true);
            return false;
        }
        
        logger.LogInformation($"{environment.ToUpper()}: docker compose started successfully.", true);
        logger.LogInformation($"{environment.ToUpper()}: new version is deployed successfully. Deployed images: {PrintDeployedImages(imageNumbersResult.Value)}", true, true);
        
        return true;
    }
    
    private static string PrintDeployedImages(Dictionary<string, string> deployedImages)
    {
        return string.Join(", ", deployedImages.Select(kvp => $"{kvp.Key}:{kvp.Value}"));
    }
}