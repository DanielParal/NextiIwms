namespace Nexticz.Lib.Shared.DeploymentInfo;

/// <summary>
/// Deployment info section is built via a deployment tool in staging and production environments.
/// So we can dynamically set it every deployment.
/// In the Dev environment, it is set manually in appsettings.json.
/// </summary>
internal class DeploymentInfoSettings
{
    public DateTimeOffset LastDeploymentTimestamp { get; set; }
}