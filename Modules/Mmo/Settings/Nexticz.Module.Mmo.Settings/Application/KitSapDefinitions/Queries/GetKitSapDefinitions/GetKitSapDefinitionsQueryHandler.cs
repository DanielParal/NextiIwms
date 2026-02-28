using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;


namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Queries.GetKitSapDefinitions;

internal class GetKitSapDefinitionsQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository
) : IRequestHandler<GetKitSapDefinitionsQuery, FilteredResult<KitSapDefinition>>
{
    public async Task<FilteredResult<KitSapDefinition>> Handle(GetKitSapDefinitionsQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<KitSapDefinition>(request.FilteringParams, cancellationToken);
    }
}