using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record BatchContract(
    [property: Required] Guid Id,
    Guid? SisterBatchId,
    [property: Required] bool HasSisterBatch,
    [property: Required] string DepositorCode,
    [property: Required] string KitCode,
    [property: Required] string KitNumber,
    [property: Required] int KitsCount,
    [property: Required] int KitsFinished,
    [property: Required] int KitsLeft,
    [property: Required] string PackagingCode,
    [property: Required] decimal PackagingHeight,
    [property: Required] string DefiningPackagingCode,
    [property: Required] string KitSapDefinitionCode,
    [property: Required] TimeSpan OptimalKitDuration,
    [property: Required] TimeSpan OptimalBatchDuration,
    [property: Required] BatchStatusContract Status
    );