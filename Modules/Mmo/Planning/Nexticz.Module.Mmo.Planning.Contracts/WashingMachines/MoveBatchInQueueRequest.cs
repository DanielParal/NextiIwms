using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record MoveBatchInQueueRequest(
    [property: Required] string LineQueueCode,
    [property: Required] InQueueMovementContract Movement);