using Nexticz.Module.Vh.Contracts.LoadedActivities;
using Nexticz.Module.Vh.Contracts.WorkerShifts;
using Nexticz.Module.Vh.Domain.LoadedActivities;

namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadedActivities.Mappers;

public static class LoadedActivitiesMappers
{
    public static LoadedActivityResponse MapToLoadedActivityResponse(this LoadedActivity loadedActivity)
    {
        return new LoadedActivityResponse
        {
            Id = loadedActivity.Id,
            Start = loadedActivity.Start,
            ActivityState = Enum.Parse<ActivityState>(loadedActivity.ActivityState.ToString()),
            ActivitySource = Enum.Parse<ActivitySource>(loadedActivity.ActivitySource.ToString()),
            WorkerCode = loadedActivity.WorkerCode
        };
    }
}