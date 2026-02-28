using MediatR;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;


namespace Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Queries.GetPackagingCirculations;

internal class GetPackagingCirculationsQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository
) : IRequestHandler<GetPackagingCirculationsQuery, FilteredResult<PackagingCirculation>>
{
    public async Task<FilteredResult<PackagingCirculation>> Handle(GetPackagingCirculationsQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<PackagingCirculation>(request.FilteringParams, cancellationToken);
    }
}