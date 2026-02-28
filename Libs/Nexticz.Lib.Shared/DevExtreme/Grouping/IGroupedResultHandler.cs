namespace Nexticz.Lib.Shared.DevExtreme.Grouping;

public interface IGroupedResultHandler
{
    Task<FilteredResult<object>> GetGroupedResultAsync<T>(BaseFilteringParams filteringParams, CancellationToken cancellationToken) where T : notnull;
}