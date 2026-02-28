using Nexticz.Module.Vh.Contracts.Centers;
using Nexticz.Module.Vh.Domain.Centers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Centers.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface ICentersRepository
{
    Task<Center?> GetCenterByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Center?> GetCenterByCodeAsync(string code, CancellationToken cancellationToken);
    Task<CenterResponse?> GetCenterResponseByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<FilteredResult> GetCentersAsync(CentersFilteringParams filteringParams, CancellationToken cancellationToken);

    Task<FilteredResult> GetCentersResponseAsync(CentersFilteringParams filteringParams,
        CancellationToken cancellationToken);
}