using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypes;

internal class GetPackagingTypesQueryHandler (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)
    : IRequestHandler<GetPackagingTypesQuery, FilteredResult<PackagingType>>
{
    public async Task<FilteredResult<PackagingType>> Handle(GetPackagingTypesQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<PackagingType>(request.FilteringParams, cancellationToken);
    }
}