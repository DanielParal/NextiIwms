using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;


namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformations;

internal class GetSpecialInformationsQueryHandler (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)
    : IRequestHandler<GetSpecialInformationsQuery, FilteredResult<SpecialInformation>>
{
    public async Task<FilteredResult<SpecialInformation>> Handle(GetSpecialInformationsQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<SpecialInformation>(request.FilteringParams, cancellationToken); 
    }
}