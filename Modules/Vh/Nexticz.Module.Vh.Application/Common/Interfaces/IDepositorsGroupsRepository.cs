using Nexticz.Module.Vh.Contracts.DepositorsGroups;
using Nexticz.Module.Vh.Domain.DepositorsGroups;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.DepositorsGroups.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface IDepositorsGroupsRepository
{
    Task<DepositorsGroup?> GetDepositorsGroupByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<DepositorsGroupResponse?> GetDepositorGroupResponseByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<FilteredResult> GetDepositorsGroupsAsync(DepositorsGroupsFilteringParams filteringParams, CancellationToken cancellationToken);
    
}