using Nexticz.Module.Vh.Contracts.WorkerShifts;

namespace Nexticz.Module.Vh.Contracts.LoadedActivities;

public class LoadedActivityResponse
{
    public required Guid Id { get; set; }
    public required DateTime Start { get; set; }
    public required string WorkerCode { get; set; }
    public required ActivityState ActivityState { get; set; }
    public required ActivitySource ActivitySource { get; set; }
}

