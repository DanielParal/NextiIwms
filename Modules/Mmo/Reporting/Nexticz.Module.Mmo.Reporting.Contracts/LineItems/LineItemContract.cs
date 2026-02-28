using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Reporting.Contracts.LineItems;

public record LineItemContract(
    [property: Required] Guid Id,
    [property: Required] string WashingMachineCode,
    [property: Required] string LineCode, 
    [property: Required] string LineCodeSuffix, 
    [property: Required] DateTimeOffset StartDate, 
    [property: Required] DateTimeOffset EndDate, 
    [property: Required] bool IsPlanned,
    [property: Required] LineItemTypeContract Type,
    [property: Required] bool HasSister,
    [property: Required] bool AffectProductivity,
    Guid? InactivityReasonId,
    string? Comment,
    Guid? BatchId, 
    Guid? KitId, 
    Guid? SisterKitId, 
    string? KitCode, 
    string? PackagingCode, 
    int? OptimalPackagingSpeedOnWashingMachine, 
    SpeedLevelContract? OptimalPackagingSpeedOnWashingMachineLevel, 
    string? KitNumber, 
    int? KitOrderId, 
    int? TotalPlannedKitsCountInBatch, 
    int? WashingMachineSpeed, 
    SpeedLevelContract? WashingMachineSpeedLevel, 
    string? KitEfficiency, 
    TimeSpan? OptimalKitDuration,
    string? DeclaredBy,
    string? UpdatedBy);