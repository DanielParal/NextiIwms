using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Module.Mmo.Settings.Contracts.Kits.Queries;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitByCode;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitResponseByCode;

internal class GetKitResponseByCodeQueryHandler(
    ISender sender, KitResponseFactory kitResponseFactory) 
    : IRequestHandler<GetKitResponseByCodeQuery, ErrorOr<KitResponse>>
{
    public async Task<ErrorOr<KitResponse>> Handle(GetKitResponseByCodeQuery request, CancellationToken cancellationToken)
    {
        var kit = await sender.Send(new GetKitByCodeQuery(request.Code), cancellationToken);
        
        if (kit.IsError)
            return kit.Errors;
        
        return await kitResponseFactory.CreateAsync(kit.Value, cancellationToken);
    }
}