using System.Text.Json;
using System.Text.Json.Serialization;
using JasperFx.Events;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Lib.Shared.DataAccess.Marten.Models;

public abstract class EventResponse
{
    protected EventResponse(IEvent rawEvent)
    {
        Id = rawEvent.Id;
        StreamId = rawEvent.StreamId;
        Version = rawEvent.Version;
        EventType = rawEvent.EventTypeName;
        TimestampOffset = rawEvent.Timestamp;
        UserName = rawEvent.Headers is not null &&
                   rawEvent.Headers.TryGetValue(MartenEventHeaderName.UserName, out var userName)
            ? userName.ToString()
            : null;
        CorrelationId =
            rawEvent.Headers is not null &&
            rawEvent.Headers.TryGetValue(MartenEventHeaderName.CorrelationId, out var correlationId)
                ? correlationId.ToString()
                : string.Empty;
        Data = JsonSerializer.Serialize(rawEvent.Data,
            new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            });
    }
    
    public Guid Id { get; set; }
    public Guid StreamId { get; set; }
    public long Version { get; set; }
    public string EventType { get; set; }
    public DateTimeOffset TimestampOffset { get; set; }
    public DateTimeOffset ExecutedAt => TimestampOffset.UtcDateTime;
    public string? UserName { get; set; }
    public string? CorrelationId { get; set; }
    public string Data { get; set; }
}