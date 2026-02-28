namespace Nexticz.OnPremise.Deployment.Cli.FileBuilders.EnvFileBuild;

internal interface IEnvFileBuilder
{
    void AddEnvSecrets(Dictionary<string, string> envSecrets);
    void BuildEnvFile();
}