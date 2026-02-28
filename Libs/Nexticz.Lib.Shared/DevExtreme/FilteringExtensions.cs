using DevExtreme.AspNet.Data.ResponseModel;

namespace Nexticz.Lib.Shared.DevExtreme;

public static class FilteringExtensions
{
    public static FilteredResult MapToFilteredResult(this LoadResult loadResult)
    {
        return new FilteredResult
        {
            data = loadResult.data,
            summary = loadResult.summary,
            groupCount = loadResult.groupCount,
            totalCount = loadResult.totalCount
        };
    }
    
    public static FilteredResult<T> MapToFilteredResult<T>(this LoadResult loadResult)
    {
        return new FilteredResult<T>
        {
            Data = loadResult.data.Cast<T>().ToList(),
            Summary = loadResult.summary,
            GroupCount = loadResult.groupCount,
            TotalCount = loadResult.totalCount
        };
    }
    
    
    public static FilteredResult<TOut> MapDataFromTInToTOut<TIn, TOut>(
        this FilteredResult<TIn> source, Func<TIn, TOut> convertFunc)
    {
        return new FilteredResult<TOut>
        {
            Data = source.Data.Select(convertFunc).ToList(),
            Summary = source.Summary,
            GroupCount = source.GroupCount,
            TotalCount = source.TotalCount
        };
    }
    
    public static FilteredResult<TOut> MapDataFromTInToTOut<TIn, TOut>(
        this FilteredResult<TIn> source, IEnumerable<TOut> data)
    {
        return new FilteredResult<TOut>
        {
            Data = data.ToList(),
            Summary = source.Summary,
            GroupCount = source.GroupCount,
            TotalCount = source.TotalCount
        };
    }
}