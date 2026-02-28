using Nexticz.Module.Vh.Contracts.WorkerShifts;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using ActivitySource = Nexticz.Module.Vh.Contracts.WorkerShifts.ActivitySource;
using ActivityType = Nexticz.Module.Vh.Contracts.WorkerShifts.ActivityType;

namespace Nexticz.Module.Vh.Presentation.Endpoints.WorkerShifts.Mappers;

public static class WorkerShiftMappers
{
    public static WorkerShiftResponse MapToWorkerShiftResponse(this WorkerShift workerShift)
    {
        return new WorkerShiftResponse
        {
            Id = workerShift.Id,
            Start = workerShift.Start,
            End = workerShift.End,
            WorkerCode = workerShift.WorkerCode,
            Approved = workerShift.Approved,
            ManualStart = workerShift.ManualStart,
            ManualEnd = workerShift.ManualEnd,
            WorkerCenterCode = workerShift.WorkerCenterCode,
            ActivityAfterCutOffCode = workerShift.ActivityAfterCutOffCode,
            Activities = workerShift.Activities.Select(x => x.MapToWorkerShiftActivityResponse()).ToList()
        };
    }

    private static WorkerShiftActivityResponse MapToWorkerShiftActivityResponse(
        this WorkerShiftActivity workerShiftActivity)
    {
        return new WorkerShiftActivityResponse
        {
            Id = workerShiftActivity.Id,
            WorkerShiftId = workerShiftActivity.WorkerShiftId,
            Start = workerShiftActivity.Start,
            End = workerShiftActivity.End,
            WorkerCode = workerShiftActivity.WorkerCode,
            CenterCode = workerShiftActivity.CenterCode,
            ActivityCode = workerShiftActivity.ActivityCode,
            DepositorCode = workerShiftActivity.DepositorCode,
            DepositorGroupCode = workerShiftActivity.DepositorGroupCode,
            ActivitySystemType = workerShiftActivity.ActivitySystemType,
            ActivityCutOff = workerShiftActivity.ActivityCutOff,
            ActivityType = Enum.Parse<ActivityType>(workerShiftActivity.ActivityType.ToString()),
            ActivitySource = Enum.Parse<ActivitySource>(workerShiftActivity.ActivitySource.ToString()),
            LoadingDeviceId = workerShiftActivity.LoadingDeviceId,
            Note = workerShiftActivity.Note,
            ActivityName = workerShiftActivity.ActivityName,
            ActivitiesCount = workerShiftActivity.ActivitiesCount,
            LastAddedActivityStart = workerShiftActivity.LastAddedActivityStart,
            Unit = workerShiftActivity.Unit,
            Coefficient = workerShiftActivity.Coefficient,
            Score = workerShiftActivity.Score,
            ActivityCountOrDurationMinutes = workerShiftActivity.ActivityCountOrDurationMinutes,
            DurationMinutes = workerShiftActivity.DurationMinutes
        };
    }
}