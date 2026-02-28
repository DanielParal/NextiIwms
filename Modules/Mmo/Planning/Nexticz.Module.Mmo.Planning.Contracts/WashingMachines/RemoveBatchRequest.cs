using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record RemoveBatchRequest(
    [property: Required] string LineQueueCode);