using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record WashingMachineResponse(
    [property: Required] string Code,
    [property: Required] bool IsHelpNeeded,
    [property: Required] WashingMachineStatusContract Status,
    [property: Required] WashingMachineLineQueueContract[] Queues);