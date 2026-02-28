namespace Nexticz.OnPremise.Deployment.Cli.FileBuilders.FeaturesJsonBuild;

internal interface IFeaturesJsonFileBuilder
{
    void AddEnvFeatureJson(Dictionary<string, string> keyValueSecrets);
    void BuildFeaturesJsonFile();
}