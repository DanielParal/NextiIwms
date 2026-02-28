using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.KitTypes.Queries.GetKitTypeByCode;

internal class GetKitTypeByCodeQueryHandler(
    IKitTypeReadOnlyRepository kitTypeReadOnlyRepository
)
    : IRequestHandler<GetKitTypeByCodeQuery, ErrorOr<KitType>>
{
    public async Task<ErrorOr<KitType>> Handle(GetKitTypeByCodeQuery request, CancellationToken cancellationToken)
    {
        var kitType = await kitTypeReadOnlyRepository.GetByCodeAsync(request.Code, cancellationToken);

        if (kitType is null) 
            return KitTypeErrors.CodeDoesNotExist;
        
        return kitType;
    }
}