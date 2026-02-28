using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Washing.Contracts.Batches;

public record BatchContract(
    [property: Required] Guid Id,
    Guid? SisterBatchId,
    [property: Required] string WashingMachineCode,
    [property: Required] string LineCode,
    [property: Required] string KitCode,
    [property: Required] string KitNumber,
    [property: Required] string PackagingCode,
    [property: Required] string DefiningPackagingCode,
    [property: Required] TimeSpan OptimalKitDuration,
    [property: Required] KitWashCycleContract[] KitWashCycles,
    [property: Required] PrintingContract[] Printings);