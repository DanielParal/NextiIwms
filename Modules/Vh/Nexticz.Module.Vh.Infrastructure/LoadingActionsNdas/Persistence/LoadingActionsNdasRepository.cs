using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.LoadingActionsNdas.Common.Models;
using Nexticz.Module.Vh.Contracts.LoadingActionsNdas;
using Nexticz.Module.Vh.Domain.LoadingActionsNdas;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.LoadingActionsNdas.Persistence;

public class LoadingActionsNdasRepository(DataContext context) : ILoadingActionsNdasRepository
{
    public async Task<LoadingActionsNda?> GetLoadingActionsNdaByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.LoadingActionsNdas
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<LoadingActionsNdaResponse?> GetLoadingActionsNdaResponseByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.LoadingActionsNdas
            .Where(x => x.Id == id)
            .Select(x => new LoadingActionsNdaResponse
            {
                Id = x.Id,
                Created = x.Created,
                Note = x.Note,
                WorkerSlug = x.WorkerSlug,
                LoadingDeviceId = x.LoadingDeviceId,
                NonDispensingActivitySlug = x.NonDispensingActivitySlug
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FilteredResult> GetLoadingActionsNdasAsync(LoadingActionsNdasFilteringParams filteringParams, CancellationToken cancellationToken)
    {
        var query = context.LoadingActionsNdas
            .Select(x => new LoadingActionsNdaResponse
            {
                Id = x.Id,
                Created = x.Created,
                Note = x.Note,
                WorkerSlug = x.WorkerSlug,
                LoadingDeviceId = x.LoadingDeviceId,
                NonDispensingActivitySlug = x.NonDispensingActivitySlug
            });

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}