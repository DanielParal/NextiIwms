using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByLineQueueCode;

internal class GetWashingMachineByLineQueueCodeQueryHandler(IWashingMachineReadOnlyRepository washingMachineReadOnlyRepository)
    : IRequestHandler<GetWashingMachineByLineQueueCodeQuery, ErrorOr<WashingMachine>>
{
    public async Task<ErrorOr<WashingMachine>> Handle(GetWashingMachineByLineQueueCodeQuery request, CancellationToken cancellationToken)
    {
        var washingMachine = await washingMachineReadOnlyRepository.GetByLineQueueCodeAsync(request.LineQueueCode, cancellationToken);
        
        if (washingMachine is null)
            return WashingMachineErrors.NotFoundWashingMachineWithLineQueueCode;
        
        return washingMachine;
    }
}