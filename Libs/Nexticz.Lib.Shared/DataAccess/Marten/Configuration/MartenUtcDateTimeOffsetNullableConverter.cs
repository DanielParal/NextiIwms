using Newtonsoft.Json;

namespace Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

public class MartenUtcDateTimeOffsetNullableConverter : JsonConverter<DateTimeOffset?>
{
    public override void WriteJson(JsonWriter writer, DateTimeOffset? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }
        
        writer.WriteValue(value.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00"));
    }

    public override DateTimeOffset? ReadJson(JsonReader reader, Type objectType, DateTimeOffset? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var raw = reader.Value?.ToString();
        if (string.IsNullOrEmpty(raw))
            return null;

        return DateTimeOffset.Parse(raw);
    }
}