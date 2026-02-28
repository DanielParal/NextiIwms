using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Washing.Contracts.Batches;

public record FinishKitResponse(
    [property: Required] KitWashCycleContract KitWashCycle);