using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.WashingMachineSpeeds.Queries.GetCurrentWashingMachineSpeedByCode;

internal class GetCurrentWashingMachineSpeedByCodeQueryHandler(
    IWashingMachineSpeedReadOnlyRepository washingMachineSpeedReadOnlyRepository) 
    : IRequestHandler<GetCurrentWashingMachineSpeedByCodeQuery, ErrorOr<WashingMachineSpeed>>
{
    public async Task<ErrorOr<WashingMachineSpeed>> Handle(GetCurrentWashingMachineSpeedByCodeQuery request, CancellationToken cancellationToken)
    {
        var washingMachineSpeed = await washingMachineSpeedReadOnlyRepository.GetCurrentWashingMachineSpeedByCodeAsync(request.Code, cancellationToken);
        
        if (washingMachineSpeed is null)
            return WashingMachineSpeedErrors.SpeedNotFound;
        
        return washingMachineSpeed;
    }
}