using Nexticz.Module.Vh.Contracts.Depositors;
using Nexticz.Module.Vh.Domain.Depositors;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Depositors.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface IDepositorsRepository
{
    Task<Depositor?> GetDepositorByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<DepositorResponse?> GetDepositorResponseByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<FilteredResult> GetDepositorsAsync(DepositorsFilteringParams filteringParams,
        CancellationToken cancellationToken);
}