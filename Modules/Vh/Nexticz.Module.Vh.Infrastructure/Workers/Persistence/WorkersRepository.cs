using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.Workers.Common.Models;
using Nexticz.Module.Vh.Contracts.Workers;
using Nexticz.Module.Vh.Domain.Workers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.Workers.Persistence;

public class WorkersRepository(DataContext context) : IWorkersRepository
{
    public async Task<Worker?> GetWorkerByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Workers
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WorkerResponse?> GetWorkerResponseByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Workers
            .Where(x => x.Id == id)
            .Select(x => new WorkerResponse
            {
                Id = x.Id,
                Name = x.Name,
                Surname = x.Surname,
                CodeWms = x.CodeWms,
                CodeSag = x.CodeSag,
                ActivityAfterCutOffCode = x.ActivityAfterCutOffCode,
                CenterId = x.CenterId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WorkerResponse?> GetWorkerResponseBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return await context.Workers
            .Where(x => x.CodeWms == slug)
            .Select(x => new WorkerResponse
            {
                Id = x.Id,
                Name = x.Name,
                Surname = x.Surname,
                CodeWms = x.CodeWms,
                CodeSag = x.CodeSag,
                ActivityAfterCutOffCode = x.ActivityAfterCutOffCode,
                CenterId = x.CenterId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FilteredResult> GetWorkersAsync(WorkersFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.Workers
            .Select(x => new WorkerResponse
            {
                Id = x.Id,
                Name = x.Name,
                Surname = x.Surname,
                CodeWms = x.CodeWms,
                CodeSag = x.CodeSag,
                ActivityAfterCutOffCode = x.ActivityAfterCutOffCode,
                CenterId = x.CenterId
            });

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}