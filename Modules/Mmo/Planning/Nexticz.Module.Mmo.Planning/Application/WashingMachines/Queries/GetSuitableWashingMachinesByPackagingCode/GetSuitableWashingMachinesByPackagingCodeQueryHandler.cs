using MediatR;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetFilteredWashingMachineResponsesByPackagingCode;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetSuitableWashingMachinesByPackagingCode;

internal class GetSuitableWashingMachinesByPackagingCodeQueryHandler(
    ISender sender,
    IWashingMachineReadOnlyRepository washingMachineReadOnlyRepository) 
    : IRequestHandler<GetSuitableWashingMachinesByPackagingCodeQuery, WashingMachine[]>
{
    public async Task<WashingMachine[]> Handle(GetSuitableWashingMachinesByPackagingCodeQuery request, CancellationToken cancellationToken)
    {
        var washingMachineResponses = await sender.Send(new GetFilteredWashingMachineResponsesByPackagingCodeQuery(request.PackagingCode, request.SisterPackagingCode), cancellationToken);
        var washingMachines = 
            await washingMachineReadOnlyRepository.GetAllByCodesAsync(
                washingMachineResponses.Select(x => x.Code).ToArray(), 
                cancellationToken);
        return washingMachines.ToArray();
    }
}