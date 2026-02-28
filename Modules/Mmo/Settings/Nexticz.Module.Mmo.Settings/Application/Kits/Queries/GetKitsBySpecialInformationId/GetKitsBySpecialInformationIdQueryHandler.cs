using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsBySpecialInformationId;

internal class GetKitsBySpecialInformationIdQueryHandler(
    IKitReadOnlyRepository kitReadOnlyRepository) 
    : IRequestHandler<GetKitsBySpecialInformationIdQuery, Kit[]>
{
    public async Task<Kit[]> Handle(GetKitsBySpecialInformationIdQuery request, CancellationToken cancellationToken)
    {
        var kits = await kitReadOnlyRepository.GetKitsBySpecialInformationId(request.SpecialInformationId, cancellationToken);
        return kits.ToArray();
    }
}