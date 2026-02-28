using System.Text.Json.Serialization;

namespace Nexticz.Module.Sign.Settings.Contracts.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ConstantTypeContract
{
    String,
    Int32,
    Boolean,
    Double,
    DateTimeOffset,
    Decimal
}