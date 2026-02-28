using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitByCode;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationsByIds;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitSpecialInformationResponsesByKitCode;

internal class GetKitSpecialInformationResponsesByKitCodeQueryHandler(
    ISender sender)
    : IRequestHandler<GetKitSpecialInformationResponsesByKitCodeQuery, KitSpecialInformationResponse[]>
{
    public async Task<KitSpecialInformationResponse[]> Handle(GetKitSpecialInformationResponsesByKitCodeQuery request, CancellationToken cancellationToken)
    {
        var kit = await sender.Send(new GetKitByCodeQuery(request.KitCode), cancellationToken);
        
        if (kit.IsError)
            return [];
        
        var specialInformationIds = kit.Value.SpecialInformationSchedules.Select(x => x.Id).ToArray();
        var specialInformations = await sender.Send(new GetSpecialInformationsByIdsQuery(specialInformationIds), cancellationToken);
        
        var kitSpecialInformations = new List<KitSpecialInformationResponse>();

        foreach (var schedule in kit.Value.SpecialInformationSchedules)
        {
            var specialInfo = specialInformations.FirstOrDefault(x => x.Id == schedule.Id);
            
            var kitSpecialInformation = KitSpecialInformationResponseFactory.Create(schedule, specialInfo);
            kitSpecialInformations.Add(kitSpecialInformation);
        }
        
        return kitSpecialInformations.ToArray();
    }
}