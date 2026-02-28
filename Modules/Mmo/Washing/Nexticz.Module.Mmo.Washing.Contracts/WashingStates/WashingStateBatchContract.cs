using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Washing.Contracts.WashingStates;

public record WashingStateBatchContract(
    [property: Required] Guid Id,
    [property: Required] Guid? SisterBatchId,
    [property: Required] string KitCode,
    [property: Required] string PackagingCode,
    [property: Required] int FinishedKits,
    [property: Required] int PlannedKitsCount,
    [property: Required] string Efficiency,
    [property: Required] int RemainingMinutes
    );