using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByCode;

internal class GetWashingMachineByCodeQueryHandler(IWashingMachineReadOnlyRepository washingMachineReadOnlyRepository) 
    : IRequestHandler<GetWashingMachineByCodeQuery, ErrorOr<WashingMachine>>
{
    public async Task<ErrorOr<WashingMachine>> Handle(GetWashingMachineByCodeQuery request, CancellationToken cancellationToken)
    {
        var washingMachine = await washingMachineReadOnlyRepository.GetByCodeAsync(request.Code, cancellationToken);

        if (washingMachine is null)
            return WashingMachineErrors.NotFoundWashingMachineWithCode;
        
        return washingMachine;
    }
}