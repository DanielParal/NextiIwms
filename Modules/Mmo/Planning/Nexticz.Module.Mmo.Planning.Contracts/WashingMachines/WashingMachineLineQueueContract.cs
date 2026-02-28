using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record WashingMachineLineQueueContract(
    [property: Required] string Code,
    [property: Required] WashingMachineLineQueueItemContract[] Items,
    [property: Required] int BatchCount,
    [property: Required] bool IsActive,
    [property: Required] bool IsAnyBatchInWashing,
    [property: Required] bool IsAnyBatchInWashingInSisterLine,
    DateTimeOffset? LastItemEndDate
    );