namespace Nexticz.Module.Vh.Contracts.WorkerShifts;

public class WorkerShiftResponse
{
    public required Guid Id { get; set; }
    public required DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public required string WorkerCode { get; set; }
    public required bool Approved { get; set; }
    public required bool ManualStart { get; set; }
    public required bool ManualEnd { get; set; }
    public required string ActivityAfterCutOffCode { get; set; }
    public required string WorkerCenterCode { get; set; }
    public required List<WorkerShiftActivityResponse> Activities { get; set; }
}

public class WorkerShiftActivityResponse
{
    public required Guid Id { get; set; }
    public required Guid WorkerShiftId { get; set; }
    public required DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public required string WorkerCode { get; set; }
    public required string CenterCode { get; set; }
    public required string ActivityCode { get; set; }
    public string? DepositorCode { get; set; }
    public string? DepositorGroupCode { get; set; }
    public required string ActivitySystemType { get; set; }
    public TimeOnly? ActivityCutOff { get; set; }
    public required ActivityType ActivityType { get; set; }
    public required ActivitySource ActivitySource { get; set; }
    public Guid? LoadingDeviceId { get; set; }
    public string? Note { get; set; }
    public required string ActivityName { get; set; }
    public int ActivitiesCount { get; set; } = 1;
    public string? Unit { get; set; }
    public decimal Coefficient { get; set; }
    public decimal Score { get; set; }
    public decimal ActivityCountOrDurationMinutes { get; set; }
    public decimal DurationMinutes { get; set; }
    public DateTime? LastAddedActivityStart { get; set; }
}

public enum ActivityType
{
    PaidMyStock,
    PaidIwms,
    PaidDynamics,
    NonProductive,
    Unknown,
    Break,
    WorkerShiftEnd
}

public enum ActivitySource
{
    MyStock,
    Iwms,
    ShiftMaster,
    Dynamics
}

public enum ActivityState
{
    Copied,
    Processed,
    ProcessedError
}

