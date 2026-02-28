using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Drying.Domain.KitAggregate.Events;

public record KitToDryingSectionTransferredEvent(
    Guid Id,
    DateTimeOffset TransferredAt) : IMartenEvent;