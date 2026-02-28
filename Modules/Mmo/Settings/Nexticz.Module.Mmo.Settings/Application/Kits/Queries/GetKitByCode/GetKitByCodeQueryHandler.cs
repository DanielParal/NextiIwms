using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitByCode;

internal class GetKitByCodeQueryHandler (IKitReadOnlyRepository kitReadOnlyRepository)
    : IRequestHandler<GetKitByCodeQuery, ErrorOr<Kit>>
{
    public async Task<ErrorOr<Kit>> Handle(GetKitByCodeQuery request, CancellationToken cancellationToken)
    {
        var kit = await kitReadOnlyRepository.GetByCodeAsync(request.Code, cancellationToken);
        
        if (kit is null)
            return KitErrors.CodeDoesNotExist;
        
        return kit;
    }
}