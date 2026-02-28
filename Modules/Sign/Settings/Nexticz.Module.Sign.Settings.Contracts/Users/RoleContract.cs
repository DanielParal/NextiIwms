using System.Text.Json.Serialization;

namespace Nexticz.Module.Sign.Settings.Contracts.Users;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoleContract
{
    SignMember
}