using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

public record BatchCreatedEvent(
    Guid BatchId,
    string WashingMachineCode,
    string LineQueueCode,
    string DepositorCode,
    string KitCode,
    string KitNumber,
    int KitsCount,
    string PackagingCode,
    decimal PackagingHeight,
    string DefiningPackagingCode,
    string KitSapDefinitionCode,
    TimeSpan OptimalKitDuration) : IMartenEvent;