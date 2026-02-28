using Nexticz.Module.Vh.Domain.LoadedActivities;
using Nexticz.Module.Vh.Domain.WorkerShifts;

namespace Nexticz.Module.Vh.Infrastructure.Common.BackgroundWorkers.Mappers;

public static class LoadedActivityMappers
{
    public static WorkerShiftActivity MapToWorkerShiftActivity(this LoadedActivity loadedActivity)
    {
        return new WorkerShiftActivity(
            loadedActivity.Start,
            loadedActivity.WorkerCode,
            loadedActivity.CenterCode,
            loadedActivity.ActivityCode,
            loadedActivity.DepositorCode,
            loadedActivity.DepositorGroupCode,
            loadedActivity.ActivitySystemType,
            loadedActivity.ActivityType,
            loadedActivity.ActivitySource
        )
        {
            LoadingDeviceId = loadedActivity.LoadingDeviceId,
            Note = loadedActivity.Note,
            ActivityCutOff = loadedActivity.ActivityCutOff,
            Unit = loadedActivity.Unit,
            Coefficient = loadedActivity.Coefficient,
            ActivityName = loadedActivity.ActivityName,
            LicenceKod = loadedActivity.LicenceKod,
            PDoklad = loadedActivity.PDoklad,
            SortKod = loadedActivity.SortKod,
        };
    }
}