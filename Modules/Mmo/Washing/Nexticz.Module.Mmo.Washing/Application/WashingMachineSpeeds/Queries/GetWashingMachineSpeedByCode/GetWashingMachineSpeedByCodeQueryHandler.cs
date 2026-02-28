using MediatR;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Queries.GetWashingMachineSpeedByCode;

internal class GetWashingMachineSpeedByCodeQueryHandler(
    IWashingMachineSpeedReadOnlyRepository readOnlyRepository) : IRequestHandler<GetWashingMachineSpeedByCodeQuery, WashingMachineSpeed?>
{
    public async Task<WashingMachineSpeed?> Handle(GetWashingMachineSpeedByCodeQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyRepository.GetWashingMachineSpeedByCodeAsync(request.Code, cancellationToken);
    }
}