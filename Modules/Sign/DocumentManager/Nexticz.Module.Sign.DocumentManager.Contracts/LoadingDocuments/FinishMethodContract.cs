using System.Text.Json.Serialization;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FinishMethodContract
{
    Signed,
    ManuallyUploaded,
    Deleted
}