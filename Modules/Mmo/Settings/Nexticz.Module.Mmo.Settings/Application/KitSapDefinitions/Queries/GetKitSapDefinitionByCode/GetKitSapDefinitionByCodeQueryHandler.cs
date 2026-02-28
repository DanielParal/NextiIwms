using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;

namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Queries.GetKitSapDefinitionByCode;

internal class GetKitSapDefinitionByCodeQueryHandler(
    IKitSapDefinitionReadOnlyRepository kitSapDefinitionReadOnlyRepository
)
    : IRequestHandler<GetKitSapDefinitionByCodeQuery, ErrorOr<KitSapDefinition>>
{
    public async Task<ErrorOr<KitSapDefinition>> Handle(GetKitSapDefinitionByCodeQuery request, CancellationToken cancellationToken)
    {
        var kitType = await kitSapDefinitionReadOnlyRepository.GetByCodeAsync(request.Code, cancellationToken);

        if (kitType is null) 
            return KitSapDefinitionErrors.CodeDoesNotExist;
        
        return kitType;
    }
}