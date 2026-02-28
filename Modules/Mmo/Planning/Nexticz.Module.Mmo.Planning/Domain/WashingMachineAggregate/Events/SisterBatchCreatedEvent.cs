using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

public record SisterBatchCreatedEvent(
    Guid BatchId,
    Guid SisterBatchId,
    string WashingMachineCode,
    string LineQueueCode,
    string SisterLineQueueCode,
    string DepositorCode,
    string KitCode,
    string KitNumber,
    string KitSapDefinitionCode,
    int KitsCount,
    string PackagingCode,
    decimal PackagingHeight,
    string SisterPackagingCode,
    decimal SisterPackagingHeight,
    string DefiningPackagingCode,
    TimeSpan OptimalKitDuration,
    TimeSpan OptimalSisterKitDuration) : IMartenEvent;