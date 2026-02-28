using System.ComponentModel.DataAnnotations.Schema;

namespace Nexticz.Module.Vh.Domain.WorkerShifts;

public class WorkerShiftActivity
{
    private WorkerShiftActivity()
    {
    }

    public WorkerShiftActivity(
        DateTime start,
        string workerCode,
        string centerCode,
        string activityCode,
        string? depositorCode,
        string? depositorGroupCode,
        string activitySystemType,
        ActivityType activityType,
        ActivitySource activitySource)
    {
        if (start == default)
            throw new ArgumentException("Start can not be minimal datetime value", nameof(start));
        SetStart(start);
        WorkerCode = workerCode;
        CenterCode = centerCode;
        ActivityCode = activityCode;
        DepositorCode = depositorCode;
        DepositorGroupCode = depositorGroupCode;
        ActivitySystemType = activitySystemType;
        ActivityType = activityType;
        ActivitySource = activitySource;

        CalculateMetaData();
    }

    public Guid Id { get; set; }
    public Guid WorkerShiftId { get; set; }
    public DateTime Start { get; private set; }
    public DateTime? End { get; private set; }
    public string WorkerCode { get; set; }
    public string CenterCode { get; set; }
    public string ActivityCode { get; set; }
    public string? DepositorCode { get; set; }
    public string? DepositorGroupCode { get; set; }
    public string ActivitySystemType { get; set; }
    public TimeOnly? ActivityCutOff { get; set; }
    public ActivityType ActivityType { get; set; }
    public ActivitySource ActivitySource { get; set; }
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
    public string? ActivityDetails { get; set; }
    [NotMapped]
    public string? LicenceKod { get; set; }
    [NotMapped]
    public string? PDoklad { get; set; }
    [NotMapped]
    public string? SortKod { get; set; }

    [NotMapped]
    private List<ActivityDetailsItem> ActivityDetailsItems
    {
        get
        {
            if (string.IsNullOrEmpty(ActivityDetails))
            {
                return [];
            }

            var items = ActivityDetails.Split(';', StringSplitOptions.RemoveEmptyEntries);

            return items.Select(itemStr =>
            {
                var parts = itemStr.Split('|', 2);
                return new ActivityDetailsItem
                {
                    PDoklad = parts.Length > 0 ? parts[0] : "",
                    SortKod = parts.Length > 1 ? parts[1] : ""
                };
            }).ToList();
        }
        set
        {
            if (value.Count == 0)
            {
                ActivityDetails = string.Empty;
                return;
            }

            ActivityDetails = string.Join(";", value.Select(x => $"{x.PDoklad}|{x.SortKod}"));
        }
    }

    public void SetStart(DateTime start)
    {
        Start = start;
        CalculateMetaData();
    }

    public void SetEnd(DateTime? end)
    {
        End = end;
        CalculateMetaData();
    }

    private void CalculateMetaData()
    {
        DurationMinutes = GetActivityDurationMinutes();
        ActivityCountOrDurationMinutes = GetActivityCountOrDurationMinutes();
        Score = GetActivityScore();
    }

    private decimal GetActivityScore()
    {
        return ActivityCountOrDurationMinutes * Coefficient;
    }

    private decimal GetActivityCountOrDurationMinutes()
    {
        return Unit == "min"
            ? DurationMinutes
            : ActivitiesCount;
    }

    private decimal GetActivityDurationMinutes()
    {
        if (End is null)
            return 0;

        var difference = (DateTime)End - Start;

        return (decimal)difference.TotalMinutes;
    }

    public override string ToString()
    {
        return $"Start: {Start:G}, End: {End:G}, Aktivita: {ActivityCode}, Pracovnik: {WorkerCode}";
    }

    public void IncreaseActivitiesCount(WorkerShiftActivity newActivity)
    {
        if (LicenceKod == "STD" && 
            ActivityCode == "BALENI" &&
            ActivityDetailsItems.Any(x=> x.PDoklad == newActivity.PDoklad && x.SortKod == newActivity.SortKod))
        {
            return;
        }

        if (LicenceKod == "STD" && 
            ActivityCode == "BALENI")
        {
            var updatedList = ActivityDetailsItems;
            updatedList.Add(new ActivityDetailsItem{PDoklad = newActivity.PDoklad, SortKod = newActivity.SortKod});
            
            ActivityDetailsItems = updatedList;
        }
        
        ActivitiesCount++;
    }
}

public class ActivityDetailsItem
{
    public string? PDoklad { get; set; }
    public string? SortKod { get; set; }
}