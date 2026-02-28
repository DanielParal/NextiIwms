using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record ActivateBatchRequest([property: Required] string LineQueueCode);