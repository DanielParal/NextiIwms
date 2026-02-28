using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;

public record SpecialInformationConfirmedEvent(
    Guid BatchId, 
    Guid SpecialInformationId,
    string WorkerName,
    DateTimeOffset DateConfirmed) : IMartenEvent;