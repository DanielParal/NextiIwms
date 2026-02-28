namespace Nexticz.Lib.Shared.DevExtreme;

public class HistoryEventsFilteringParams : BaseFilteringParams
{
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}