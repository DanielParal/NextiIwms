using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate.Events;

public record DriedKitCreatedEvent(
    Guid Id,
    Guid KitIdFromDrying,
    int CompletedKitsCount,
    Guid BatchId,
    string KitCode,
    string LineCode,
    int ExpectedDryingTime,
    DateTimeOffset DryingStarted,
    DateTimeOffset DryingEnded,
    DriedKitDestination Destination,
    DateTimeOffset? TransferredToDryingSectionAt) : IMartenEvent;