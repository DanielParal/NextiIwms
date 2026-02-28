using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;


namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.Queries.GetDepositorGroups;

internal class GetDepositorGroupsQueryHandler
    (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetDepositorGroupsQuery, FilteredResult<DepositorGroup>>
{
    public async Task<FilteredResult<DepositorGroup>> Handle(GetDepositorGroupsQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<DepositorGroup>(request.FilteringParams, cancellationToken);
    }
}