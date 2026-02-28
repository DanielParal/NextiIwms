using Nexticz.Module.Vh.Contracts.WorkerShifts;

namespace Nexticz.Module.Vh.Contracts.ShiftMasterChanges;

public class ShiftMasterChangesResponse
{
    public Guid Id { get; set; }
    public required string User { get; set; }
    public required DateTime Date { get; set; }
    public required ShiftMasterActivityType ShiftMasterActivityType { get; set; }
    public required string WorkerCode { get; set; }
    public required string CenterCode { get; set; }
    public required Guid WorkerShiftId { get; set; }
    public required List<WorkerShiftChangeResponse> WorkerShiftChanges { get; set; }
    public required List<WorkerShiftActivityChangeResponse> WorkerShiftActivityChanges { get; set; }
}

public class WorkerShiftChangeResponse
{
    public required Guid Id { get; set; }
    public required Guid WorkerShiftId { get; set; }
    public required ContextChangeType ContextChangeType { get; set; }
    public required DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public required string WorkerCode { get; set; }
    public required string WorkerCenterCode { get; set; }
    public required bool Approved { get; set; }
}

public class WorkerShiftActivityChangeResponse
{
    public required Guid Id { get; set; }
    public required Guid WorkerShiftActivityId { get; set; }
    public required ContextChangeType ContextChangeType { get; set; }
    public required Guid WorkerShiftId { get; set; }
    public required DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public required string WorkerCode { get; set; }
    public required string CenterCode { get; set; }
    public required string ActivityCode { get; set; }
    public TimeOnly? ActivityCutOff { get; set; }
    public required ActivityType ActivityType { get; set; }
    public required ActivitySource ActivitySource { get; set; }
    public Guid? LoadingDeviceId { get; set; }
    public string? Note { get; set; }
    public required int ActivitiesCount { get; set; } = 1;
    public string? Unit { get; set; }
    public required decimal Coefficient { get; set; }
    public required decimal Score { get; set; }
    public required decimal ActivityCountOrDurationMinutes { get; set; }
    public required decimal DurationMinutes { get; set; }
    public DateTime? LastAddedActivityStart { get; set; }
}

public enum ShiftMasterActivityType
{
    AddWorkerShift,
    AddActivity,
    EndWorkerShift,
    ApproveWorkerShift,
    DisApproveWorkerShift
}

public enum ContextChangeType
{
    Added,
    ChangedFrom,
    ChangedTo,
    Removed
}