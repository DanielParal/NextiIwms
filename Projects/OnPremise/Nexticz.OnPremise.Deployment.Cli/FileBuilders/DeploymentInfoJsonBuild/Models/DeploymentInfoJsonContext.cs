using System.Text.Json.Serialization;

namespace Nexticz.OnPremise.Deployment.Cli.FileBuilders.DeploymentInfoJsonBuild.Models;

[JsonSerializable(typeof(DeploymentInfoContainer))]
[JsonSourceGenerationOptions(WriteIndented = true)]
internal partial class DeploymentInfoJsonContext : JsonSerializerContext
{
}