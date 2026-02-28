namespace Nexticz.OnPremise.Deployment.Cli.FileBuilders.SecretsJsonBuild;

internal interface ISecretsJsonFileBuilder
{
    void AddEnvSecretJson(Dictionary<string, string> keyValueSecrets);
    void BuildSecretsJsonFile();
}