using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;

public record SisterBatchDetachedEvent(
    Guid Id, 
    string WashingMachineCode,
    string LineCode) : IMartenEvent;