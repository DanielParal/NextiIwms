using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.HistoryEvents;

public record HistoryEventResponse(
    [property: Required] Guid Id, 
    [property: Required] Guid StreamId,
    [property: Required] long Version,
    [property: Required] string EventType,
    [property: Required] DateTimeOffset ExecutedAt,
    [property: Required] string? UserName,
    [property: Required] string? CorrelationId,
    [property: Required] string Data);