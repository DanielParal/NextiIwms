using Nexticz.OnPremise.Deployment.Cli.AzureKeyVault;
using Nexticz.OnPremise.Deployment.Cli.Coordinators;
using Nexticz.OnPremise.Deployment.Cli.FileBuilders.DeploymentInfoJsonBuild;
using Nexticz.OnPremise.Deployment.Cli.FileBuilders.EnvFileBuild;
using Nexticz.OnPremise.Deployment.Cli.FileBuilders.FeaturesJsonBuild;
using Nexticz.OnPremise.Deployment.Cli.FileBuilders.SecretsJsonBuild;
using Nexticz.OnPremise.Deployment.Cli.VersionUpdate;
using Microsoft.Extensions.DependencyInjection;

namespace Iwms_deployment_cli;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoordinates(this IServiceCollection services)
    {
        return services.AddSingleton<IDisplayMenuCoordinator, DisplayMenuCoordinator>()
            .AddSingleton<IUpdateVersionsCoordinator, UpdateVersionsCoordinator>()
            .AddSingleton<IDeploymentCoordinator, DeploymentCoordinator>()
            .AddSingleton<IApplicationCoordinator, ApplicationCoordinator>();
    }
    
    public static IServiceCollection AddVersionUpdateStep(this IServiceCollection services)
    {
        return services.AddSingleton<IAzureAcrVersionGetter, AzureAcrVersionGetter>()
            .AddSingleton<IVersionBuilder, VersionBuilder>();
    }
    
    public static IServiceCollection AddFileBuildsStep(this IServiceCollection services)
    {
        return services.AddSingleton<IAzureKeyVaultDownloader, AzureKeyVaultDownloader>()
            .AddSingleton<IEnvFileBuilder, EnvFileBuilder>()
            .AddSingleton<ISecretsJsonFileBuilder, SecretsJsonFileBuilder>()
            .AddSingleton<IImageNumberExtractor, ImageNumberExtractor>()
            .AddSingleton<IDeploymentInfoJsonFileBuilder, DeploymentInfoJsonFileBuilder>()
            .AddSingleton<IFeaturesJsonFileBuilder, FeaturesJsonFileBuilder>();
    }
}