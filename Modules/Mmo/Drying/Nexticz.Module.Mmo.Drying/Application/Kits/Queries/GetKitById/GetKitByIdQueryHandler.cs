using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Drying.Application.Interfaces;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Drying.Application.Kits.Queries.GetKitById;

internal class GetKitByIdQueryHandler(
    IDryingReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetKitByIdQuery, ErrorOr<Kit>>
{
    public async Task<ErrorOr<Kit>> Handle(GetKitByIdQuery request, CancellationToken cancellationToken)
    {
        var kit = await readOnlyEventStoreRepository.GetByIdAsync<Kit>(request.Id, cancellationToken);

        if (kit is null)
            return KitErrors.KitNotFound;

        return kit;
    }
}