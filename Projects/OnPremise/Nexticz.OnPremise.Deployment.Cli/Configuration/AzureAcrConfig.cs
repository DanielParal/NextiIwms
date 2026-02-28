namespace Nexticz.OnPremise.Deployment.Cli.Configuration;

internal class AzureAcrConfig
{
    public string RegistryName { get; set; }
    public Repository[] Repositories { get; set; }
    
    internal class Repository
    {
        public string ImageName { get; set; }
        public string LocalEnvKey { get; set; }
    }
}