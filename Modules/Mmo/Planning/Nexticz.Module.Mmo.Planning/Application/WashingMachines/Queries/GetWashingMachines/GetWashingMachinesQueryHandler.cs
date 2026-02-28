using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachines;

internal class GetWashingMachinesQueryHandler(
    IWashingMachineReadOnlyRepository washingMachineReadOnlyRepository,
    ISender sender)
    : IRequestHandler<GetWashingMachinesQuery, WashingMachine[]>
{
    public async Task<WashingMachine[]> Handle(GetWashingMachinesQuery request, CancellationToken cancellationToken)
    {
        var washingMachineFromSettings = 
            await sender.Send(new GetWashingMachineResponsesQuery(), cancellationToken);
        
        var washingMachines = 
            await washingMachineReadOnlyRepository.GetAllByCodesAsync(
                washingMachineFromSettings.Select(x => x.Code).ToArray(), 
                cancellationToken);
        
        return washingMachines.ToArray();
    }
}