using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record ChangeBatchKitsCountRequest(
    [property: Required] string LineQueueCode,
    [property: Required] int KitsCountToChange);