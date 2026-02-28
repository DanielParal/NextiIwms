using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Module.Mmo.Settings.Contracts.Kits.Queries;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitByCode;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationById;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetCurrentKitSpecialInformationResponseByKitCode;

internal class GetCurrentKitSpecialInformationResponseByKitCodeQueryHandler(
    ISender sender,
    IClock clock)
    : IRequestHandler<GetCurrentKitSpecialInformationResponseByKitCodeQuery, KitSpecialInformationResponse?>
{
    public async Task<KitSpecialInformationResponse?> Handle(GetCurrentKitSpecialInformationResponseByKitCodeQuery request, CancellationToken cancellationToken)
    {
        var kit = await sender.Send(new GetKitByCodeQuery(request.KitCode), cancellationToken);
        
        if (kit.IsError)
            return null;
        
        var now = clock.UtcNowOffset;
        var currentSpecialInformationSchedule = kit.Value
            .SpecialInformationSchedules.FirstOrDefault(x => x.StartDate <= now && now < x.EndDate);
        
        if (currentSpecialInformationSchedule is null)
            return null;
        
        var specialInformation = await sender.Send(new GetSpecialInformationByIdQuery(currentSpecialInformationSchedule.Id), cancellationToken);
        
        if (specialInformation.IsError)
            return KitSpecialInformationResponseFactory.Create(currentSpecialInformationSchedule, null);;
        
        return KitSpecialInformationResponseFactory.Create(currentSpecialInformationSchedule, specialInformation.Value);
    }
}