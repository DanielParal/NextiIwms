using Nexticz.Module.Vh.Domain.ReportActivities;
using Nexticz.Module.Vh.Domain.ReportPerformanceEvaluations;

namespace Nexticz.Module.Vh.Domain.WorkerShifts;

public class WorkerShift
{
    private List<WorkerShiftActivity> _activities = [];

    private WorkerShift()
    {
    }

    public WorkerShift(string workerCenterCode, WorkerShiftActivity activity)
    {
        if (activity is null)
            throw new ArgumentNullException(nameof(activity), "New WorkerShift must have one Activity");

        if (activity.ActivityType == ActivityType.WorkerShiftEnd)
            throw new ArgumentException("New WorkerShift must not have Activity with type WorkerShiftEnd");

        Start = activity.Start;

        WorkerCode = activity.WorkerCode;

        WorkerCenterCode = workerCenterCode;

        _activities.Add(activity);
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public string WorkerCode { get; set; }
    public string WorkerCenterCode { get; set; }
    public bool Approved { get; private set; }
    public bool ManualStart { get; set; }
    public bool ManualEnd { get; set; }
    public string? ActivityAfterCutOffCode { get; set; }
    public required string ActivityAfterCutOffName { get; set; }

    public IReadOnlyList<WorkerShiftActivity> Activities => _activities;

    public ICollection<ReportActivity> ReportActivities { get; set; }

    public ReportPerformanceEvaluation ReportPerformanceEvaluation { get; set; }

    public List<WorkerShiftActivity> CloseWorkerShift(DateTime workerShiftEndTime)
    {
        if (Approved)
            throw new InvalidOperationException("Can not close WorkerShift if WorkerShift is Approved");

        End = workerShiftEndTime;

        EndLastActivity(workerShiftEndTime);

        var trimmedActivities = _activities.Where(x => x.Start > workerShiftEndTime).ToList();

        foreach (var workerShiftActivity in trimmedActivities) _activities.Remove(workerShiftActivity);

        return trimmedActivities;
    }

    public void SetApproval(bool approval)
    {
        if (approval)
        {
            ApproveWorkerShift();
            return;
        }

        DisApproveWorkerShift();
    }

    private void ApproveWorkerShift()
    {
        if (Approved)
            throw new InvalidOperationException("WorkerShift Approved must be set to false before Approval");

        if (_activities.Any(x => x.ActivityType == ActivityType.Unknown))
            throw new InvalidOperationException("Approved WorkerShift must not have any unknown Activity");

        if (End is null)
            throw new InvalidOperationException("Approved WorkerShift must have EndTime");

        Approved = true;
    }

    private void DisApproveWorkerShift()
    {
        if (Approved is not true)
            throw new InvalidOperationException("Can not disApprove WorkerShift if WorkerShift is Approved");

        Approved = false;
    }

    public void AddActivity(WorkerShiftActivity activity)
    {
        if (Approved)
            throw new InvalidOperationException("Can not add Activity if WorkerShift is Approved");

        if (WorkerCode != activity.WorkerCode)
            throw new InvalidOperationException(
                "Can not add Activity with different WorkerCode than WorkerShift WorkerCode");

        AddActivityOrSpreadExistingActivity(activity);

        // todo: workaround for STD special activity

        DeleteActivitiesWithZeroDuration();

        SortWorkerShiftActivitiesAscending();
    }

    public void CheckLastNotEndedActivityCutOff()
    {
        if (_activities.Count(x => x.End is null) > 1)
            throw new InvalidOperationException("WorkerShift can has only one not ended activity");

        var notEndedActivity = _activities.FirstOrDefault(x => x.End == null);

        if (notEndedActivity is null)
            return;

        if (notEndedActivity.ActivityType == ActivityType.Unknown)
            return;

        if (GetActivityMaxEnd(notEndedActivity) > DateTime.Now || notEndedActivity.ActivityCutOff is null)
            return;

        notEndedActivity.SetEnd(GetActivityMaxEnd(notEndedActivity));

        _activities.Add(AddUnknownActivity(GetActivityMaxEnd(notEndedActivity), null));
    }

    private void DeleteActivitiesWithZeroDuration()
    {
        var removedActivities = new List<WorkerShiftActivity>();

        foreach (var workerShiftActivity in _activities)
            if (workerShiftActivity.Start == workerShiftActivity.End)
                removedActivities.Add(workerShiftActivity);

        foreach (var workerShiftActivity in removedActivities) _activities.Remove(workerShiftActivity);
    }

    private void AddActivityOrSpreadExistingActivity(WorkerShiftActivity newActivity)
    {
        SortWorkerShiftActivitiesAscending();

        var firstActivity = _activities.First();
        var overlappingActivity = CheckIfActivityIsOverlapping(newActivity.Start);
        var lastActivity = _activities.Last();

        if (newActivity.Start < firstActivity.Start)
            AddActivityOrSpreadExistingActivityBeforeWorkerShiftStart(newActivity, firstActivity);

        if (overlappingActivity is not null)
            AddOverlappingActivityOrSpreadExistingActivity(newActivity, overlappingActivity);

        if (overlappingActivity is null && newActivity.Start >= lastActivity.Start)
            AddActivityOrSpreadExistingActivityToEnd(newActivity, lastActivity);
    }

    private void AddActivityOrSpreadExistingActivityBeforeWorkerShiftStart(WorkerShiftActivity newActivity,
        WorkerShiftActivity firstActivity)
    {
        Start = newActivity.Start;

        if (GetActivityMaxEnd(newActivity) < firstActivity.Start && newActivity.ActivityCutOff is not null)
        {
            newActivity.SetEnd(GetActivityMaxEnd(newActivity));
            _activities.Add(newActivity);
            _activities.Add(AddUnknownActivity((DateTime)newActivity.End!, firstActivity.Start));
            return;
        }

        if (HasActivitiesSameCode(newActivity, firstActivity))
        {
            firstActivity.IncreaseActivitiesCount(newActivity);
            firstActivity.SetStart(newActivity.Start);
            return;
        }

        newActivity.SetEnd(firstActivity.Start);
        _activities.Add(newActivity);
    }

    private void AddOverlappingActivityOrSpreadExistingActivity(WorkerShiftActivity newActivity,
        WorkerShiftActivity overlappingActivity)
    {
        var overlappedActivityEnd = overlappingActivity.End;

        if (GetActivityMaxEnd(newActivity) < overlappingActivity.End && newActivity.ActivityCutOff is not null)
        {
            overlappingActivity.SetEnd(newActivity.Start);
            newActivity.SetEnd(GetActivityMaxEnd(newActivity));
            _activities.Add(newActivity);
            _activities.Add(AddUnknownActivity((DateTime)newActivity.End!, (DateTime)overlappedActivityEnd!));
            return;
        }

        if (HasActivitiesSameCode(newActivity, GetNextActivity(overlappingActivity)))
        {
            var nextActivity = GetNextActivity(overlappingActivity);

            overlappingActivity.SetEnd(newActivity.Start);
            nextActivity!.IncreaseActivitiesCount(newActivity);
            nextActivity.SetStart(newActivity.Start);
            return;
        }

        WorkerShiftActivity comparedActivity;

        if (HasActivitiesSameCode(newActivity, overlappingActivity))
        {
            overlappingActivity.IncreaseActivitiesCount(newActivity);
            overlappingActivity.LastAddedActivityStart = newActivity.Start;
            overlappingActivity.SetEnd(overlappingActivity.End);
            comparedActivity = overlappingActivity;
        }
        else
        {
            overlappingActivity.SetEnd(newActivity.Start);
            newActivity.SetEnd(overlappedActivityEnd);
            _activities.Add(newActivity);
            comparedActivity = newActivity;
        }

        SortWorkerShiftActivitiesAscending();

        if (GetNextActivity(comparedActivity)?.ActivityType == ActivityType.Unknown)
            SolveUnknownActivityDuration(comparedActivity, GetNextActivity(comparedActivity)!);
    }

    private void SolveUnknownActivityDuration(WorkerShiftActivity newActivity, WorkerShiftActivity unknownActivity)
    {
        if (unknownActivity.End.HasValue && GetActivityMaxEnd(newActivity) < unknownActivity.End &&
            newActivity.ActivityCutOff is not null)
        {
            newActivity.SetEnd(GetActivityMaxEnd(newActivity));
            unknownActivity.SetStart(GetActivityMaxEnd(newActivity));
            return;
        }

        if (unknownActivity.End.HasValue && !HasActivitiesSameCode(newActivity, GetNextActivity(unknownActivity)))
        {
            newActivity.SetEnd(unknownActivity.End);
            unknownActivity.SetStart((DateTime)unknownActivity.End);
            return;
        }

        if (unknownActivity.End.HasValue)
        {
            var nextActivity = GetNextActivity(unknownActivity)!;
            nextActivity.SetStart(newActivity.Start);
            nextActivity.ActivitiesCount += newActivity.ActivitiesCount;
            nextActivity.SetEnd(nextActivity.End);
            newActivity.SetEnd(newActivity.Start);
            unknownActivity.SetEnd(unknownActivity.Start);
            return;
        }

        newActivity.SetEnd(null);
        unknownActivity.SetEnd(unknownActivity.Start);
        CheckLastNotEndedActivityCutOff();
    }

    private void AddActivityOrSpreadExistingActivityToEnd(WorkerShiftActivity newActivity,
        WorkerShiftActivity lastActivity)
    {
        End = null;

        if (GetActivityMaxEnd(lastActivity) < newActivity.Start && lastActivity.ActivityCutOff is not null)
        {
            lastActivity.SetEnd(GetActivityMaxEnd(lastActivity));
            _activities.Add(newActivity);
            _activities.Add(AddUnknownActivity((DateTime)lastActivity.End!, newActivity.Start));
            return;
        }

        if (HasActivitiesSameCode(lastActivity, newActivity))
        {
            lastActivity.IncreaseActivitiesCount(newActivity);
            lastActivity.LastAddedActivityStart = newActivity.Start;
            lastActivity.SetEnd(null);
            return;
        }

        lastActivity.SetEnd(newActivity.Start);
        _activities.Add(newActivity);
        CheckLastNotEndedActivityCutOff();
    }

    private static bool HasActivitiesSameCode(WorkerShiftActivity? activity1, WorkerShiftActivity? activity2)
    {
        return activity1?.ActivityCode == activity2?.ActivityCode && activity1?.Note is null &&
               activity2?.Note is null && activity1?.ActivityName == activity2?.ActivityName;
    }

    private DateTime GetActivityMaxEnd(WorkerShiftActivity activity)
    {
        var cutoffTimeSpan = activity.ActivityCutOff?.ToTimeSpan() ?? TimeSpan.Zero;

        if (activity.LastAddedActivityStart is not null)
            return (DateTime)activity.LastAddedActivityStart?.Add(cutoffTimeSpan)!;

        return activity.Start.Add(cutoffTimeSpan);
    }

    private WorkerShiftActivity? GetNextActivity(WorkerShiftActivity activity)
    {
        var activityIndex = _activities.IndexOf(activity);

        return activityIndex == _activities.Count - 1 ? null : _activities[activityIndex + 1];
    }

    private void SortWorkerShiftActivitiesAscending()
    {
        _activities = _activities.OrderBy(x => x.Start).ToList();
    }

    private WorkerShiftActivity AddUnknownActivity(DateTime start, DateTime? end)
    {
        var unknownActivity = new WorkerShiftActivity(start, WorkerCode, "unknown", "unknown", null, null, "unknown",
            ActivityType.Unknown,
            ActivitySource.Iwms) { ActivityName = "Neznámá" };
        unknownActivity.SetEnd(end);

        SetActivityAfterCutOff(unknownActivity);

        return unknownActivity;
    }

    private void SetActivityAfterCutOff(WorkerShiftActivity unknownActivity)
    {
        if (string.IsNullOrEmpty(ActivityAfterCutOffCode))
            return;

        unknownActivity.ActivityType = ActivityType.PaidIwms;
        unknownActivity.CenterCode = WorkerCenterCode;
        unknownActivity.ActivityCode = ActivityAfterCutOffCode;
        unknownActivity.ActivitySystemType = "Nevýdejová";
        unknownActivity.ActivityName = ActivityAfterCutOffName;
        unknownActivity.Coefficient = 1;
        unknownActivity.Unit = "min";
    }

    private WorkerShiftActivity? CheckIfActivityIsOverlapping(DateTime newActivityTime)
    {
        foreach (var workerShiftActivity in _activities)
        {
            if (workerShiftActivity.End is null)
                continue;

            if (newActivityTime >= workerShiftActivity.Start && newActivityTime < workerShiftActivity.End)
                return workerShiftActivity;
        }

        return null;
    }

    private void EndLastActivity(DateTime end)
    {
        SortWorkerShiftActivitiesAscending();

        var overlappingActivity = CheckIfActivityIsOverlapping(end);

        if (_activities.Last().End is null && _activities.Last().Start == end)
        {
            _activities.Remove(_activities.Last());
            return;
        }

        if (overlappingActivity is not null && end == overlappingActivity.Start)
        {
            _activities.Remove(overlappingActivity);
            return;
        }

        if (overlappingActivity is not null)
        {
            overlappingActivity.SetEnd(end);
            return;
        }

        if (GetActivityMaxEnd(_activities.Last()) < end && _activities.Last().ActivityCutOff is not null)
        {
            _activities.Last().SetEnd(GetActivityMaxEnd(_activities.Last()));
            _activities.Add(AddUnknownActivity(GetActivityMaxEnd(_activities.Last()), end));
            return;
        }

        _activities.Last().SetEnd(end);
    }
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