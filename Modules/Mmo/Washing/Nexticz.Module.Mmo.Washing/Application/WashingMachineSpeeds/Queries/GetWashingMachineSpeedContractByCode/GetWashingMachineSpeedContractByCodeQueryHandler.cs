using MediatR;
using Nexticz.Module.Mmo.Washing.Contracts.WashingMachineSpeeds;
using Nexticz.Module.Mmo.Washing.Contracts.WashingMachineSpeeds.Queries;
using Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Queries.GetWashingMachineSpeedByCode;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Queries.GetWashingMachineSpeedContractByCode;

internal class GetWashingMachineSpeedContractByCodeQueryHandler(ISender sender) : IRequestHandler<GetWashingMachineSpeedContractByCodeQuery, WashingMachineSpeedContract?>
{
    public async Task<WashingMachineSpeedContract?> Handle(GetWashingMachineSpeedContractByCodeQuery request, CancellationToken cancellationToken)
    {
        var washingMachineSpeed = await sender.Send(new GetWashingMachineSpeedByCodeQuery(request.Code), cancellationToken);
        return washingMachineSpeed == null ? null : new WashingMachineSpeedContract(washingMachineSpeed.Id, washingMachineSpeed.Code, washingMachineSpeed.Speed);
    }
}