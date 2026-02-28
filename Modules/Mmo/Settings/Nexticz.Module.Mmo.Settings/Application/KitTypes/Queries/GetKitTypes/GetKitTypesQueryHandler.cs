using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Application.KitTypes.Queries.GetKitTypes;

internal class GetKitTypesQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository
) : IRequestHandler<GetKitTypesQuery, FilteredResult<KitType>>
{
    public async Task<FilteredResult<KitType>> Handle(GetKitTypesQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<KitType>(request.FilteringParams, cancellationToken);
    }
}