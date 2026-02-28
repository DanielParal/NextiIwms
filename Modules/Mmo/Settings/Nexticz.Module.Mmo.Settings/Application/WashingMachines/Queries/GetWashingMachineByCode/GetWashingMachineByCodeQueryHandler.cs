using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;

namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachineByCode;

internal class GetWashingMachineByCodeQueryHandler (IWashingMachineReadOnlyRepository washingMachineReadOnlyRepository) 
    : IRequestHandler<GetWashingMachineByCodeQuery, ErrorOr<WashingMachine>>
{
    public async Task<ErrorOr<WashingMachine>> Handle(GetWashingMachineByCodeQuery request, CancellationToken cancellationToken)
    {
        var washingMachine = await washingMachineReadOnlyRepository.GetByCodeAsync(request.Code, cancellationToken);

        if (washingMachine == null)
            return WashingMachineErrors.CodeDoesNotExist;

        return washingMachine;
    }
}