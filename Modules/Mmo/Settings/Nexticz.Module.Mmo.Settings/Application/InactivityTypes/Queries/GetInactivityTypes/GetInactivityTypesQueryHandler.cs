using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Queries.GetInactivityTypes;

internal class GetInactivityTypesQueryHandler(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetInactivityTypesQuery, FilteredResult<InactivityType>>
{
    public async Task<FilteredResult<InactivityType>> Handle(GetInactivityTypesQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<InactivityType>(request.FilteringParams, cancellationToken);
    }
}