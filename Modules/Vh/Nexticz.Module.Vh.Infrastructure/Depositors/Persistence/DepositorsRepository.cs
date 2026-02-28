using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.Depositors.Common.Models;
using Nexticz.Module.Vh.Contracts.Depositors;
using Nexticz.Module.Vh.Domain.Depositors;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.Depositors.Persistence;

public class DepositorsRepository(DataContext context) : IDepositorsRepository
{
    public async Task<Depositor?> GetDepositorByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Depositors
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<DepositorResponse?> GetDepositorResponseByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Depositors
            .Where(x => x.Id == id)
            .Select(x => new DepositorResponse
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                CenterId = x.CenterId,
                DepositorsGroupId = x.DepositorsGroupId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FilteredResult> GetDepositorsAsync(DepositorsFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.Depositors
            .Select(x => new DepositorResponse
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                CenterId = x.CenterId,
                DepositorsGroupId = x.DepositorsGroupId
            });

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}