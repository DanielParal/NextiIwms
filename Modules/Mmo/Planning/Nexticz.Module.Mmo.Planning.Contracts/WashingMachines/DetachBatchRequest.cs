using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record DetachSisterBatchRequest(
    [property: Required] string LineQueueCode);