using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Contracts.KitSapDefinitions;
using Nexticz.Module.Mmo.Settings.Contracts.KitSapDefinitions.Queries;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Queries.GetKitSapDefinitionByCode;

namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Queries.GetKitSapDefinitionResponseByCode;

internal class GetKitSapDefinitionResponseByCodeQueryHandler(ISender sender) 
    : IRequestHandler<GetKitSapDefinitionResponseByCodeQuery, ErrorOr<KitSapDefinitionResponse>>
{
    public async Task<ErrorOr<KitSapDefinitionResponse>> Handle(GetKitSapDefinitionResponseByCodeQuery request, CancellationToken cancellationToken)
    {
        var kitSapDefinition = await sender.Send(new GetKitSapDefinitionByCodeQuery(request.Code), cancellationToken);
        
        if (kitSapDefinition.IsError)
            return kitSapDefinition.Errors;
        
        return KitSapDefinitionResponseFactory.Create(kitSapDefinition.Value);
    }
}