using MediatR;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachines;
using Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Queries.GetWashingMachinesTimeTables;

internal class GetWashingMachinesTimeTablesQueryHandler(
    ISender sender,
    WashingMachineTimeTableFactory washingMachineTimeTableFactory)
    : IRequestHandler<GetWashingMachinesTimeTablesQuery, WashingMachineTimeTable[]>
{
    public async Task<WashingMachineTimeTable[]> Handle(GetWashingMachinesTimeTablesQuery request, CancellationToken cancellationToken)
    {
        var washingMachines = await sender.Send(new GetWashingMachinesQuery(), cancellationToken);
        
        var washingMachinesTimeTables = await washingMachines
            .ToAsyncEnumerable()
            .SelectAwait(async washingMachine => 
                await washingMachineTimeTableFactory.CreateAsync(washingMachine, cancellationToken))
            .ToArrayAsync(cancellationToken);
        
        return washingMachinesTimeTables.OrderBy(x => x.Code).ToArray();
    }
}