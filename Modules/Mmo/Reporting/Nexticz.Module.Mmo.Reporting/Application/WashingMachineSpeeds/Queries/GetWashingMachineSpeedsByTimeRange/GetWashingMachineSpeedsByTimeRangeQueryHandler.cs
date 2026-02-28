using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.WashingMachineSpeeds.Queries.GetWashingMachineSpeedsByTimeRange;

internal class GetWashingMachineSpeedsByTimeRangeQueryHandler(
    IWashingMachineSpeedReadOnlyRepository readOnlyRepository) : IRequestHandler<GetWashingMachineSpeedsByTimeRangeQuery, WashingMachineSpeed[]>
{
    public async Task<WashingMachineSpeed[]> Handle(GetWashingMachineSpeedsByTimeRangeQuery request, CancellationToken cancellationToken)
    {
        var speeds = await readOnlyRepository.GetWashingMachineSpeedsByTimeRangeAsync(request.StartDate, request.EndDate, cancellationToken);
        return speeds.ToArray();
    }
}