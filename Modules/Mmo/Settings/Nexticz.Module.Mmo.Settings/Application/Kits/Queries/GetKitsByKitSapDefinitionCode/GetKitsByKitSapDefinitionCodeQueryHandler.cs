using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsByKitSapDefinitionCode;

internal class GetKitsByKitSapDefinitionCodeQueryHandler (IKitReadOnlyRepository kitReadOnlyRepository)
    : IRequestHandler<GetKitsByKitSapDefinitionCodeQuery, IReadOnlyList<Kit>>
{
    public async Task<IReadOnlyList<Kit>> Handle(GetKitsByKitSapDefinitionCodeQuery request, CancellationToken cancellationToken)
    {
        return await kitReadOnlyRepository.GetKitsByKitSapDefinitionCodeAsync(request.KitSapDefinitionCode, cancellationToken);
    }
}