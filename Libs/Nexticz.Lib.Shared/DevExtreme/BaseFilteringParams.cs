namespace Nexticz.Lib.Shared.DevExtreme;

public class BaseFilteringParams
{
    public BaseFilteringParams() { }

    protected BaseFilteringParams(BaseFilteringParams source)
    {
        Skip = source.Skip;
        Take = source.Take;
        RequireTotalCount = source.RequireTotalCount;
        RequireGroupCount = source.RequireGroupCount;
        Sort = source.Sort;
        Filter = source.Filter;
        TotalSummary = source.TotalSummary;
        Group = source.Group;
        GroupSummary = source.GroupSummary;
    }
    
    public string? Skip { get; set; }
    public string? Take { get; set; }
    public string? RequireTotalCount { get; set; }
    public string? RequireGroupCount { get; set; }
    public string? Sort { get; set; }
    public string? Filter { get; set; }
    public string? TotalSummary { get; set; }
    public string? Group { get; set; }
    public string? GroupSummary { get; set; }
    
    public bool IsOrderApplied => !string.IsNullOrEmpty(Sort);
}