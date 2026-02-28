using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Washing.Contracts.WashingStates;

public record WashingStateLineContract(
    [property: Required] string Code,
    [property: Required] int Speed,
    WashingStateBatchContract? Batch
    );