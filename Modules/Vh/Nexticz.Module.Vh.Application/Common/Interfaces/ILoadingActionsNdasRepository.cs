using Nexticz.Module.Vh.Contracts.LoadingActionsNdas;
using Nexticz.Module.Vh.Domain.LoadingActionsNdas;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.LoadingActionsNdas.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface ILoadingActionsNdasRepository
{
    Task<LoadingActionsNda?> GetLoadingActionsNdaByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<LoadingActionsNdaResponse?> GetLoadingActionsNdaResponseByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<FilteredResult> GetLoadingActionsNdasAsync(LoadingActionsNdasFilteringParams filteringParams,
        CancellationToken cancellationToken);
}