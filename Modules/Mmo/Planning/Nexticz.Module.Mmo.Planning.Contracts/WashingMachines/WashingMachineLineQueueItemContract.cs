using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record WashingMachineLineQueueItemContract(
    [property: Required] WashingMachineLineQueueItemTypeContract TypeContract,
    [property: Required] DateTimeOffset StartDate,
    [property: Required] DateTimeOffset EndDate,
    Guid? Id,
    string? KitCode,
    string? PackagingCode,
    string? SisterPackagingCode,
    bool? IsSisterPackaging,
    int? KitsCount,
    int? KitsFinished,
    int? Index,
    int? SisterBatchIndex,
    BatchStatusContract? Status
    );