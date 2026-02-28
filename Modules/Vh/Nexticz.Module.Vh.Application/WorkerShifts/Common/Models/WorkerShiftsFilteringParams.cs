using Nexticz.Lib.Shared.DevExtreme;


namespace Nexticz.Module.Vh.Application.WorkerShifts.Common.Models;

public class WorkerShiftsFilteringParams : BaseFilteringParams
{
    public int? FromYear { get; set; }
    public int? FromMonth { get; set; }
    public DateTime? ActivitiesStart { get; set; }
    public DateTime? ActivitiesEnd { get; set; }
    public string? ActivityCenter { get; set; }
}