using Nexticz.Module.Vh.Contracts.ShiftMasterChanges;
using Nexticz.Module.Vh.Contracts.WorkerShifts;
using Nexticz.Module.Vh.Domain.ShiftMasterChanges;
using ShiftMasterActivityType = Nexticz.Module.Vh.Contracts.ShiftMasterChanges.ShiftMasterActivityType;

namespace Nexticz.Module.Vh.Presentation.Endpoints.ShiftMasterChanges.Mappers;

public static class ShiftMasterChangesMappers
{
    public static ShiftMasterChangesResponse MapToShiftMasterChangesResponse(this ShiftMasterChange shiftMasterChange)
    {
        return new ShiftMasterChangesResponse
        {
            Id = shiftMasterChange.Id,
            Date = shiftMasterChange.Date,
            User = shiftMasterChange.User,
            CenterCode = shiftMasterChange.CenterCode,
            WorkerCode = shiftMasterChange.WorkerCode,
            WorkerShiftId = shiftMasterChange.WorkerShiftId,
            ShiftMasterActivityType =
                Enum.Parse<ShiftMasterActivityType>(shiftMasterChange.ShiftMasterActivityType.ToString()),
            WorkerShiftChanges = shiftMasterChange.WorkerShiftChanges.Select(x => x.MapToWorkerShiftChangeResponse())
                .ToList(),
            WorkerShiftActivityChanges = shiftMasterChange.WorkerShiftActivityChanges
                .Select(x => x.MapToWorkerShiftActivityChangeResponse()).ToList()
        };
    }

    private static WorkerShiftChangeResponse MapToWorkerShiftChangeResponse(this WorkerShiftChange workerShiftChange)
    {
        return new WorkerShiftChangeResponse
        {
            Id = workerShiftChange.Id,
            Start = workerShiftChange.Start,
            End = workerShiftChange.End,
            WorkerCode = workerShiftChange.WorkerCode,
            WorkerCenterCode = workerShiftChange.WorkerCenterCode,
            WorkerShiftId = workerShiftChange.WorkerShiftId,
            Approved = workerShiftChange.Approved,
            ContextChangeType = Enum.Parse<ContextChangeType>(workerShiftChange.ContextChangeType.ToString())
        };
    }

    private static WorkerShiftActivityChangeResponse MapToWorkerShiftActivityChangeResponse(
        this WorkerShiftActivityChange workerShiftActivityChange)
    {
        return new WorkerShiftActivityChangeResponse
        {
            Id = workerShiftActivityChange.Id,
            WorkerShiftId = workerShiftActivityChange.WorkerShiftId,
            Start = workerShiftActivityChange.Start,
            End = workerShiftActivityChange.End,
            WorkerCode = workerShiftActivityChange.WorkerCode,
            CenterCode = workerShiftActivityChange.CenterCode,
            ActivityCode = workerShiftActivityChange.ActivityCode,
            ActivityCutOff = workerShiftActivityChange.ActivityCutOff,
            ActivityType = Enum.Parse<ActivityType>(workerShiftActivityChange.ActivityType.ToString()),
            ActivitySource = Enum.Parse<ActivitySource>(workerShiftActivityChange.ActivitySource.ToString()),
            LoadingDeviceId = workerShiftActivityChange.LoadingDeviceId,
            Note = workerShiftActivityChange.Note,
            ActivitiesCount = workerShiftActivityChange.ActivitiesCount,
            LastAddedActivityStart = workerShiftActivityChange.LastAddedActivityStart,
            Unit = workerShiftActivityChange.Unit,
            Coefficient = workerShiftActivityChange.Coefficient,
            Score = workerShiftActivityChange.Score,
            ActivityCountOrDurationMinutes = workerShiftActivityChange.ActivityCountOrDurationMinutes,
            DurationMinutes = workerShiftActivityChange.DurationMinutes,
            ContextChangeType = Enum.Parse<ContextChangeType>(workerShiftActivityChange.ContextChangeType.ToString()),
            WorkerShiftActivityId = workerShiftActivityChange.WorkerShiftActivityId
        };
    }
}