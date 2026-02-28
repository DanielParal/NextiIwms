using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record FinishBatchRequest(
    [property: Required] string LineQueueCode);