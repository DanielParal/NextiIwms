using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.Helpers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;


namespace Nexticz.Lib.Shared.Helpers;

public static class FilteringHelper
{
    public static DataSourceLoadOptionsBase CreateLoadOptionsFromFilteringParams(BaseFilteringParams filteringParams)
    {
        var loadOptions = new DataSourceLoadOptionsBase();

        DataSourceLoadOptionsParser.Parse(loadOptions, key =>
        {
            var obj = filteringParams.GetType().GetProperty(key.ToFirstLetterToUpper());
            var val = obj?.GetValue(filteringParams);

            return obj != null && val != null
                ? val.ToString()
                : null;
        });

        loadOptions.StringToLower = true;
        return loadOptions;
    }
}