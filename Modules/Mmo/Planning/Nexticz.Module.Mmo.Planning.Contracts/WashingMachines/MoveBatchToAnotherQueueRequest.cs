using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record MoveBatchToAnotherQueueRequest(
    [property: Required] string CurrentLineQueueCode,
    [property: Required] string NewLineQueueCode);