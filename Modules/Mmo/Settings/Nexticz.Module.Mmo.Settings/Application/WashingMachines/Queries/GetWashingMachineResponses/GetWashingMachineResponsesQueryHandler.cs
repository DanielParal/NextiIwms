using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachines;


namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachineResponses;

internal class GetWashingMachineResponsesQueryHandler(ISender sender)
    : IRequestHandler<GetWashingMachineResponsesQuery, WashingMachineResponse[]>
{
    public async Task<WashingMachineResponse[]> Handle(GetWashingMachineResponsesQuery request, CancellationToken cancellationToken)
    {
        var washingMachines = await sender.Send(new GetWashingMachinesQuery(new BaseFilteringParams()), cancellationToken);

        return washingMachines.Data.Select(WashingMachineResponseFactory.Create).ToArray();
    }
}