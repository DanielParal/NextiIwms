using Nexticz.Module.Vh.Contracts.WorkerShifts;

namespace Nexticz.Module.Vh.Contracts.LoadedActivities;

public class UpdateLoadedActivityRequest
{
    public required string WorkerCode { get; set; }
    public required ActivityState ActivityState { get; set; }
}