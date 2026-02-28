using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Washing.Contracts.Batches;

public record CreatePrintingRequest(
    [property: Required] string LineCode,
    [property: Required] Guid KitWashCycleId);