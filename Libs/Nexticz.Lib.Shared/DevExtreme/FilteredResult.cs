using DevExtreme.AspNet.Data.ResponseModel;

namespace Nexticz.Lib.Shared.DevExtreme;

public class FilteredResult : LoadResult
{
}

public class FilteredResult<T>
{
    public required List<T> Data { get; set; }
    public object[] Summary { get; set; }
    public required int GroupCount { get; set; }
    public required int TotalCount { get; set; }
}