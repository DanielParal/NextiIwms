using MediatR;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Queries.GetWashingMachineSpeeds;

internal class GetWashingMachineSpeedsQueryHandler(
    IWashingReadOnlyEventStoreRepository washingReadOnlyEventStoreRepository) : IRequestHandler<GetWashingMachineSpeedsQuery, WashingMachineSpeed[]>
{
    public async Task<WashingMachineSpeed[]> Handle(GetWashingMachineSpeedsQuery request, CancellationToken cancellationToken)
    {
        var speeds = await washingReadOnlyEventStoreRepository.GetAllAsync<WashingMachineSpeed>(cancellationToken);
        return speeds.ToArray();
    }
}