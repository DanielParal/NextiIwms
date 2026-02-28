using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Depositors.Queries.GetDepositors;

internal class GetDepositorsQueryHandler(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetDepositorsQuery, FilteredResult<Depositor>>
{
    public async Task<FilteredResult<Depositor>> Handle(GetDepositorsQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<Depositor>(request.FilteringParams, cancellationToken);
    }
}