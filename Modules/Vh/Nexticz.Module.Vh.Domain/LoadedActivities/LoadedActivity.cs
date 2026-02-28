using Nexticz.Module.Vh.Domain.WorkerShifts;

namespace Nexticz.Module.Vh.Domain.LoadedActivities;

public class LoadedActivity(
    DateTime created,
    DateTime start,
    string workerCode,
    string centerCode,
    string activityCode,
    string? depositorCode,
    string? depositorGroupCode,
    string activitySystemType,
    ActivityType activityType,
    ActivitySource activitySource,
    ActivityState activityState,
    bool pickingPlaceHight)
{
    public Guid Id { get; set; }
    public DateTime Created { get; set; } = created;
    public DateTime Start { get; set; } = start;
    public DateTime? End { get; set; }
    public string WorkerCode { get; set; } = workerCode;
    public string CenterCode { get; set; } = centerCode;
    public string ActivityCode { get; set; } = activityCode;
    public string? DepositorCode { get; set; } = depositorCode;
    public string? DepositorGroupCode { get; set; } = depositorGroupCode;
    public string ActivitySystemType { get; set; } = activitySystemType;
    public TimeOnly? ActivityCutOff { get; set; }
    public ActivityType ActivityType { get; set; } = activityType;
    public ActivitySource ActivitySource { get; set; } = activitySource;
    public ActivityState ActivityState { get; set; } = activityState;
    public Guid? LoadingDeviceId { get; set; }
    public string? Note { get; set; }
    public required string ActivityName { get; set; }
    public string? Unit { get; set; }
    public decimal Coefficient { get; set; }
    public int ActivitiesCount { get; set; } = 1;
    public int? Idd { get; set; }
    public int? Idt { get; set; }
    public int? Idi { get; set; }
    public int? Idp { get; set; }
    public bool PickingPlaceHight { get; set; } = pickingPlaceHight;
    public string? PDoklad { get; set; }
    public string? SortKod { get; set; }
    public string? LicenceKod { get; set; }

    public override string ToString()
    {
        return $"Start: {Start:G}, Aktivita: {ActivityCode}, Pracovnik: {WorkerCode}";
    }
}