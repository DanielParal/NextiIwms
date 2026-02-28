using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsByManufactureCode;

internal class GetKitsByManufactureCodeQueryHandler (IKitReadOnlyRepository kitReadOnlyRepository) 
    : IRequestHandler<GetKitsByManufactureCodeQuery, IReadOnlyList<Kit>>
{
    public async Task<IReadOnlyList<Kit>> Handle(GetKitsByManufactureCodeQuery request, CancellationToken cancellationToken)
    {
        return await kitReadOnlyRepository.GetKitsByManufactureCodeAsync(request.ManufactureCode, cancellationToken);
    }
}