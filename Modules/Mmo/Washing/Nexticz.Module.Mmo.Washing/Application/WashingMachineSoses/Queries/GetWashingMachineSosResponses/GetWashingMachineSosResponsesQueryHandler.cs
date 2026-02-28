using MediatR;
using Nexticz.Module.Mmo.Washing.Contracts.WashingMachineSoses;
using Nexticz.Module.Mmo.Washing.Contracts.WashingMachineSoses.Queries;
using Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Queries.GetWashingMachineSoses;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Queries.GetWashingMachineSosResponses;

internal class GetWashingMachineSosResponsesQueryHandler(ISender sender) : IRequestHandler<GetWashingMachineSosResponsesQuery, WashingMachineSosResponse[]>
{
    public async Task<WashingMachineSosResponse[]> Handle(GetWashingMachineSosResponsesQuery request, CancellationToken cancellationToken)
    {
        var washingMachineSoses = await sender.Send(new GetWashingMachineSosesQuery(), cancellationToken);
        return washingMachineSoses.Select(WashingMachineSosResponseFactory.Create).ToArray();
    }
}