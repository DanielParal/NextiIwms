using System.Text.Json.Serialization;

namespace Nexticz.Module.Mmo.Settings.Contracts.Users;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReceivableNotificationContract
{
    SosCalled,
    SosResolved
}