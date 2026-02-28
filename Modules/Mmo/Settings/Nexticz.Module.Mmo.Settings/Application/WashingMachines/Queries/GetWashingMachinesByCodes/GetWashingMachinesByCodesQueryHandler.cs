using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;

namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachinesByCodes;

internal class GetWashingMachinesByCodesQueryHandler (IWashingMachineReadOnlyRepository washingMachineReadOnlyRepository) 
    : IRequestHandler<GetWashingMachinesByCodesQuery, IReadOnlyList<WashingMachine>>
{
    public async Task<IReadOnlyList<WashingMachine>> Handle(GetWashingMachinesByCodesQuery request, CancellationToken cancellationToken)
    {
        return await washingMachineReadOnlyRepository.GetWashingMachinesByCodesAsync(request.Codes, cancellationToken);
    }
}