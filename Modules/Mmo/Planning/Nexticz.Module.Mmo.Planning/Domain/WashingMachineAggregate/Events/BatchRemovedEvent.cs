using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

public record BatchRemovedEvent(
    Guid BatchId,
    string WashingMachineCode,
    string LineQueueCode) : IMartenEvent;