using System.Text.Json.Serialization;

namespace Nexticz.Module.Sign.Settings.Contracts.Users;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PermissionContract
{
   SignManageAll,
   SignManageSettings,
   SignManageDocumentManager,
   SignDeviceManageDocuments,
   SignManageDeleteDocuments
}