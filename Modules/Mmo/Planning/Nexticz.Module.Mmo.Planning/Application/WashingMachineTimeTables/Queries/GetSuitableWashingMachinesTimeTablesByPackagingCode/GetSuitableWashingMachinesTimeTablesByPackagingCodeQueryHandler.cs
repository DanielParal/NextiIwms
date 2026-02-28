using MediatR;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetSuitableWashingMachinesByPackagingCode;
using Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Queries.GetSuitableWashingMachinesTimeTablesByPackagingCode;

internal class GetSuitableWashingMachinesTimeTablesByPackagingCodeQueryHandler (
    ISender sender,
    WashingMachineTimeTableFactory washingMachineTimeTableFactory)
    : IRequestHandler<GetSuitableWashingMachinesTimeTablesByPackagingCodeQuery, WashingMachineTimeTable[]>
{
    public async Task<WashingMachineTimeTable[]> Handle(GetSuitableWashingMachinesTimeTablesByPackagingCodeQuery request, CancellationToken cancellationToken)
    {
        var washingMachines = await sender.Send(
            new GetSuitableWashingMachinesByPackagingCodeQuery(request.PackagingCode, request.SisterPackagingCode), 
            cancellationToken);
        
        var washingMachinesTimeTables = await washingMachines
            .ToAsyncEnumerable()
            .SelectAwait(async washingMachine => 
                await washingMachineTimeTableFactory.CreateAsync(washingMachine, cancellationToken))
            .ToArrayAsync(cancellationToken);
        
        return washingMachinesTimeTables.OrderBy(x => x.Code).ToArray();
    }
}