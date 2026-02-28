using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Drying.Application.Interfaces;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate;


namespace Nexticz.Module.Mmo.Drying.Application.Kits.Queries.GetKits;

internal class GetKitsQueryHandler(
    IDryingReadOnlyEventStoreRepository dryingReadOnlyRepository) : IRequestHandler<GetKitsQuery, FilteredResult<Kit>>
{
    public async Task<FilteredResult<Kit>> Handle(GetKitsQuery request, CancellationToken cancellationToken)
    {
        return await dryingReadOnlyRepository.GetFilteredAsync<Kit>(request.FilteringParams, cancellationToken);
    }
}