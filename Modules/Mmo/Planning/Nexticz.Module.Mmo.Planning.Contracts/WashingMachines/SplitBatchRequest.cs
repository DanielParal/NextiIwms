using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record SplitBatchRequest(
    [property: Required] string LineQueueCode,
    [property: Required] int KitsCountToChange,
    [property: Required] int KitsCountToCreate);