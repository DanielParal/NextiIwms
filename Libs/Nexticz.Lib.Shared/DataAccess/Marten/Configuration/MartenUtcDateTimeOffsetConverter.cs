using Newtonsoft.Json;

namespace Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

public class MartenUtcDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset ReadJson(JsonReader reader, Type objectType, DateTimeOffset existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return DateTimeOffset.Parse(reader.Value?.ToString() ?? string.Empty);
    }

    public override void WriteJson(JsonWriter writer, DateTimeOffset value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00"));
    }
}