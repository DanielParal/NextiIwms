using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Manufactures.Queries.GetManufactures;

internal class GetManufacturesQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository
) : IRequestHandler<GetManufacturesQuery, FilteredResult<Manufacture>>
{
    public async Task<FilteredResult<Manufacture>> Handle(GetManufacturesQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<Manufacture>(request.FilteringParams, cancellationToken);
    }
}