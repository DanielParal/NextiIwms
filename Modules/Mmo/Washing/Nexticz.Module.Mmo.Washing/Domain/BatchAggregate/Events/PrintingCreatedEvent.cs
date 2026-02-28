using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Washing.Domain.PrintingEntity;

namespace Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;

public record PrintingCreatedEvent(
    Guid Id,
    Guid BatchId,
    Guid KitId,
    DateTimeOffset DatePrinted,
    PrintingStatus Status,
    PrintingType Type,
    string? FailureReason) : IMartenEvent;