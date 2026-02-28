using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachineByCode;

namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachineResponseByCode;

public class GetWashingMachineResponseByCodeQueryHandler(ISender sender) 
    : IRequestHandler<GetWashingMachineResponseByCodeQuery, ErrorOr<WashingMachineResponse>>
{
    public async Task<ErrorOr<WashingMachineResponse>> Handle(GetWashingMachineResponseByCodeQuery request, CancellationToken cancellationToken)
    {
        var washingMachine = await sender.Send(new GetWashingMachineByCodeQuery(request.Code), cancellationToken);
        if (washingMachine.IsError)
            return washingMachine.Errors;
        
        return WashingMachineResponseFactory.Create(washingMachine.Value);
    }
}