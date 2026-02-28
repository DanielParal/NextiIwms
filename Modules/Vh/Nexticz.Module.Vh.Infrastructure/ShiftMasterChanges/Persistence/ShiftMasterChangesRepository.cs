using DevExtreme.AspNet.Data;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.ShiftMasterChanges.Common.Models;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.ShiftMasterChanges.Persistence;

public class ShiftMasterChangesRepository(DataContext context) : IShiftMasterChangesRepository
{
    public async Task<FilteredResult> GetShiftMasterChangesAsync(ShiftMasterChangesFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.ShiftMasterChanges;

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}