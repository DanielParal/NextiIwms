using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Drying.Application.Interfaces;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Drying.Application.Kits.Queries.GetKitByCompletedKitsCount;

internal class GetKitByCompletedKitsCountQueryHandler(IKitReadOnlyRepository kitReadOnlyRepository) : IRequestHandler<GetKitByCompletedKitsCountQuery, ErrorOr<Kit>>
{
    public async Task<ErrorOr<Kit>> Handle(GetKitByCompletedKitsCountQuery request, CancellationToken cancellationToken)
    {
        var kit = await kitReadOnlyRepository.GetKitByCompletedKitsCountAsync(request.CompletedKitsCount, cancellationToken);

        if (kit is null)
            return KitErrors.KitNotFound;

        return kit;
    }
}