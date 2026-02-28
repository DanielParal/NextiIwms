using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsByKitTypeCode;

internal class GetKitsByKitTypeCodeQueryHandler (IKitReadOnlyRepository kitReadOnlyRepository)
    : IRequestHandler<GetKitsByKitTypeCodeQuery, IReadOnlyList<Kit>>
{
    public async Task<IReadOnlyList<Kit>> Handle(GetKitsByKitTypeCodeQuery request, CancellationToken cancellationToken)
    {
        return await kitReadOnlyRepository.GetKitsByKitTypeCodeAsync(request.KitTypeCode, cancellationToken);
    }
}