using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate;

public class Inactivity : AggregateRoot
{
    public Guid ShiftId { get; private set; }
    public string WashingMachineCode { get; private set; }
    public string LineCode { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public Guid? InactivityReasonId { get; private set; }
    public bool IsCommentNeededForReview { get; private set; }
    public bool AffectProductivity { get; private set; }
    public string? Comment { get; private set; }
    public InactivityType Type { get; private set; }
    public bool IsPlanned { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string? DeclaredBy { get; private set; }
    public string? UpdatedBy { get; private set; }

    public Inactivity(
        Guid shiftId,
        string washingMachineCode,
        string lineCode,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        Guid? inactivityReasonId,
        bool isCommentNeededForReview,
        bool affectProductivity,
        InactivityType type,
        bool isPlanned,
        DateTimeOffset createdAt,
        string? declaredBy,
        string? updatedBy = null,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        ShiftId = shiftId;
        WashingMachineCode = washingMachineCode;
        LineCode = lineCode;
        StartDate = startDate;
        EndDate = endDate;
        InactivityReasonId = inactivityReasonId;
        IsCommentNeededForReview = isCommentNeededForReview;
        AffectProductivity = affectProductivity;
        Comment = null;
        Type = type;
        IsPlanned = isPlanned;
        CreatedAt = createdAt;
        DeclaredBy = declaredBy;
        UpdatedBy = updatedBy;
    }
}