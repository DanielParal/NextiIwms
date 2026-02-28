using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Reporting.Domain.Views;

public class LineItemView
{
    public Guid Id { get; set; }
    public Guid ShiftId { get; set; }
    public string WashingMachineCode { get; set; }
    public string LineCode { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public Guid? InactivityReasonId { get; set; }
    public bool IsCommentNeededForReview { get; set; }
    public bool AffectProductivity { get; set; }
    public LineItemType Type { get; set; }
    public bool IsPlanned { get; set; }
    public Guid? BatchId { get; set; }
    public Guid? KitId { get; set; }
    public Guid? SisterKitId { get; set; }
    public string? KitCode { get; set; }
    public string? PackagingCode { get; set; }
    public int? OptimalPackagingSpeedOnWashingMachine { get; set; }
    public SpeedLevel? OptimalPackagingSpeedOnWashingMachineLevel { get; set; }
    public string? KitNumber { get; set; }
    public int? KitOrderId { get; set; }
    public int?  TotalPlannedKitsCountInBatch { get; set; }
    public int? WashingMachineSpeed { get; set; }
    public SpeedLevel? WashingMachineSpeedLevel { get; set; }
    public double? KitEfficiency { get; set; }
    public TimeSpan? RealTimeKitDuration { get; set; }
    public TimeSpan? OptimalKitDuration { get; set; }
    public string? Comment { get; set; }
    public string? DeclaredBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public bool DoesTypeAffectsProductivityByDefault()
    {
        return DoesTypeAffectsProductivityByDefault(Type);
    }
    
    public static bool DoesTypeAffectsProductivityByDefault(LineItemType type)
    {
        return type is LineItemType.Downtime;
    }
    
    public bool IsTypeWhichMightAffectProductivity()
    {
        return IsTypeWhichMightAffectProductivity(Type);
    }
    
    public static bool IsTypeWhichMightAffectProductivity(LineItemType type)
    {
        return type is LineItemType.Shutdown or LineItemType.Downtime;
    }
}