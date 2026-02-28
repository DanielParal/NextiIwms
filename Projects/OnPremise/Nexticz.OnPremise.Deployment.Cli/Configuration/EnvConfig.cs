namespace Nexticz.OnPremise.Deployment.Cli.Configuration;

internal class EnvConfig
{
    public IwmsFeVersion[] IwmsFeVersions { get; set; }
    
    internal class IwmsFeVersion
    {
        public string MajorVersion { get; set; }
        public string ImageNumberLookupKey { get; set; }
        public string VersionKey { get; set; }
    }
}