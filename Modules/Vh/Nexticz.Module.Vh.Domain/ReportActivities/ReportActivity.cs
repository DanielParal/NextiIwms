using Nexticz.Module.Vh.Domain.WorkerShifts;

namespace Nexticz.Module.Vh.Domain.ReportActivities;

public class ReportActivity
{
    private ReportActivity()
    {
    }

    public ReportActivity(
        DateOnly date,
        Guid workerShiftId,
        DateTime workerShiftStart,
        DateTime workerShiftEnd,
        string workerCenterCode,
        string centerCode,
        string? depositorCode,
        string? depositorGroupCode,
        string activitySystemType,
        ActivitySource activitySource,
        string workerCode,
        string activityCode,
        decimal durationTime,
        int activitiesCount,
        decimal coefficient,
        decimal score,
        decimal workerShiftPowerPercentage)
    {
        Date = date;
        WorkerShiftId = workerShiftId;
        WorkerShiftStart = workerShiftStart;
        WorkerShiftEnd = workerShiftEnd;
        WorkerCenterCode = workerCenterCode;
        CenterCode = centerCode;
        DepositorCode = depositorCode;
        DepositorGroupCode = depositorGroupCode;
        ActivitySystemType = activitySystemType;
        ActivitySource = activitySource;
        WorkerCode = workerCode;
        ActivityCode = activityCode;
        DurationTime = durationTime;
        ActivitiesCount = activitiesCount;
        Coefficient = coefficient;
        Score = score;
        WorkerShiftPowerPercentage = workerShiftPowerPercentage;
    }

    public Guid Id { get; init; }
    public DateOnly Date { get; private set; }
    public Guid WorkerShiftId { get; private set; }
    public DateTime WorkerShiftStart { get; private set; }
    public DateTime WorkerShiftEnd { get; private set; }
    public string WorkerCenterCode { get; private set; }
    public string CenterCode { get; private set; }
    public string? DepositorCode { get; set; }
    public string? DepositorGroupCode { get; set; }
    public string ActivitySystemType { get; set; }
    public ActivitySource ActivitySource { get; private set; }
    public string WorkerCode { get; private set; }
    public string ActivityCode { get; private set; }
    public string? Note { get; init; }
    public decimal DurationTime { get; private set; }
    public int ActivitiesCount { get; private set; }
    public decimal Coefficient { get; private set; }
    public decimal Score { get; private set; }
    public decimal WorkerShiftPowerPercentage { get; private set; }
    public WorkerShift? WorkerShift { get; set; }
}