using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Domain.WorkerShifts;

namespace Nexticz.Module.Vh.Domain.ShiftMasterChanges;

public class WorkerShiftActivityChange
{
    private WorkerShiftActivityChange()
    {
    }

    public WorkerShiftActivityChange(
        ContextChangeType contextChangeType,
        Guid workerShiftActivityId,
        Guid workerShiftId,
        DateTime start,
        DateTime? end,
        string workerCode,
        string centerCode,
        string activityCode,
        TimeOnly? activityCutOff,
        ActivityType activityType,
        ActivitySource activitySource,
        Guid? loadingDeviceId,
        string? note,
        int activitiesCount,
        string? unit,
        decimal coefficient,
        decimal score,
        decimal activityCountOrDurationMinutes,
        decimal durationMinutes,
        DateTime? lastAddedActivityStart)
    {
        WorkerShiftActivityId = workerShiftActivityId;
        ContextChangeType = contextChangeType;
        WorkerShiftId = workerShiftId;
        Start = start;
        End = end;
        WorkerCode = workerCode;
        CenterCode = centerCode;
        ActivityCode = activityCode;
        ActivityCutOff = activityCutOff;
        ActivityType = activityType;
        ActivitySource = activitySource;
        LoadingDeviceId = loadingDeviceId;
        Note = note;
        ActivitiesCount = activitiesCount;
        Unit = unit;
        Coefficient = coefficient;
        Score = score;
        ActivityCountOrDurationMinutes = activityCountOrDurationMinutes;
        DurationMinutes = durationMinutes;
        LastAddedActivityStart = lastAddedActivityStart;
    }

    public Guid Id { get; set; }
    public Guid WorkerShiftActivityId { get; set; }
    public ContextChangeType ContextChangeType { get; set; }
    public Guid WorkerShiftId { get; set; }
    public DateTime Start { get; private set; }
    public DateTime? End { get; private set; }
    public string WorkerCode { get; set; }
    public string CenterCode { get; set; }
    public string ActivityCode { get; set; }
    public TimeOnly? ActivityCutOff { get; set; }
    public ActivityType ActivityType { get; set; }
    public ActivitySource ActivitySource { get; set; }
    public Guid? LoadingDeviceId { get; set; }
    public string? Note { get; set; }
    public int ActivitiesCount { get; set; } = 1;
    public string? Unit { get; set; }
    public decimal Coefficient { get; set; }
    public decimal Score { get; set; }
    public decimal ActivityCountOrDurationMinutes { get; set; }
    public decimal DurationMinutes { get; set; }
    public DateTime? LastAddedActivityStart { get; set; }
}