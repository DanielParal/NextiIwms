using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.ShiftMasterChanges.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface IShiftMasterChangesRepository
{
    Task<FilteredResult> GetShiftMasterChangesAsync(ShiftMasterChangesFilteringParams filteringParams,
        CancellationToken cancellationToken);
}