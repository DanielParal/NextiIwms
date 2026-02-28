using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByCode;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.UpdateWashingMachineStatus;

internal class UpdateWashingMachineStatusCommandHandler(
    IPlanningUnitOfWork unitOfWork,
    ILogger<UpdateWashingMachineStatusCommandHandler> logger,
    ISender sender)  : IRequestHandler<UpdateWashingMachineStatusCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateWashingMachineStatusCommand request, CancellationToken cancellationToken)
    {
        var upperWashingMachineCode = request.WashingMachineCode.ToUpperInvariant();
        var washingMachine = 
            await sender.Send(
                new GetWashingMachineByCodeQuery(upperWashingMachineCode), cancellationToken);

        if (washingMachine.IsError)
        {
            logger.LogWarning("Object {ObjectName} with code: {Code} does not exist. We cannot remove batch from queue.",
                nameof(washingMachine), upperWashingMachineCode);
            return WashingMachineErrors.NotFoundWashingMachineWithCode;
        }
        
        var washingMachineStatusUpdatedEvent = new WashingMachineStatusUpdatedEvent(washingMachine.Value.Code, request.Status, request.LineQueues);
        unitOfWork.AppendEvent(washingMachine.Value.Id, washingMachineStatusUpdatedEvent);

        logger.LogInformation("Washing machine updated in planning. Id: {WashingMachineId}, Code: {WashingMachineCode}, Status: {WashingMachineStatus}",
            washingMachine.Value.Id, washingMachine.Value.Code, request.Status);
        
        return Result.Updated;
    }
}