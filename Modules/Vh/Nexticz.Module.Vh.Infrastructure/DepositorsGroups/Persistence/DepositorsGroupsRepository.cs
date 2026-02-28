using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.DepositorsGroups.Common.Models;
using Nexticz.Module.Vh.Contracts.Centers;
using Nexticz.Module.Vh.Contracts.DepositorsGroups;
using Nexticz.Module.Vh.Domain.DepositorsGroups;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.DepositorsGroups.Persistence;

public class DepositorsGroupsRepository(DataContext context) : IDepositorsGroupsRepository
{
    public async Task<DepositorsGroup?> GetDepositorsGroupByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.DepositorsGroups
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<DepositorsGroupResponse?> GetDepositorGroupResponseByIdAsync(Guid id,
        CancellationToken cancellationToken)
    {
        return await context.DepositorsGroups
            .Where(x => x.Id == id)
            .Select(x => new DepositorsGroupResponse
            {
                Id = x.Id, Name = x.Name, Code = x.Code, CenterId = x.CenterId,
                Center = new CenterResponse
                {
                    Id = x.Center.Id, Name = x.Center.Name, Code = x.Center.Code,
                    ShowDashboardSalaryData = x.Center.ShowDashboardSalaryData
                }
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FilteredResult> GetDepositorsGroupsAsync(DepositorsGroupsFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.DepositorsGroups
            .Select(x => new DepositorsGroupResponse
            {
                Id = x.Id, Name = x.Name, Code = x.Code, CenterId = x.CenterId,
                Center = new CenterResponse
                {
                    Id = x.Center.Id, Name = x.Center.Name, Code = x.Center.Code,
                    ShowDashboardSalaryData = x.Center.ShowDashboardSalaryData
                }
            });

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadresult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadresult.MapToFilteredResult();
    }
}