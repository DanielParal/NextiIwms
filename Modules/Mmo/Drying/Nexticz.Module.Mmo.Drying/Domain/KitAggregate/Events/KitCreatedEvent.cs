using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Drying.Domain.KitAggregate.Events;

public record KitCreatedEvent(
    Guid Id,
    int GlobalKitsCount,
    Guid BatchId,
    string KitCode,
    string LineCode,
    int ExpectedDryingTime,
    DateTimeOffset DryingStarted) : IMartenEvent;