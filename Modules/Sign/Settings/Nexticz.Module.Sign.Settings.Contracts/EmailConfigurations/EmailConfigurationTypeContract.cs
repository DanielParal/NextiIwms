using System.Text.Json.Serialization;

namespace Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EmailConfigurationTypeContract
{
    LoadingConfiguration,
    DeliveryConfiguration,
}