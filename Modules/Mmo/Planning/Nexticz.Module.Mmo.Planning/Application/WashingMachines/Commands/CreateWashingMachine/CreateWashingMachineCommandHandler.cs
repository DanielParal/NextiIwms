using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByCode;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateWashingMachine;

internal class CreateWashingMachineCommandHandler(
    IPlanningUnitOfWork unitOfWork,
    ILogger<CreateWashingMachineCommandHandler> logger,
    ISender sender)  : IRequestHandler<CreateWashingMachineCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateWashingMachineCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await IsValidAsync(request, cancellationToken);
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var washingMachine = new WashingMachine(
            request.WashingMachineCode,
            request.Status,
            request.LineQueues);

        var createdEvent = new WashingMachineCreatedEvent(
            washingMachine.Id, 
            washingMachine.Code, 
            washingMachine.Status,
            washingMachine.IsOneLineMachine, 
            washingMachine.LineQueues.ToArray());
        
        unitOfWork.StartStream<WashingMachineCreatedEvent, WashingMachine>(
            washingMachine.Id, createdEvent);
        
        logger.LogInformation("Washing machine created in planning. Id: {WashingMachineId}, Code: {WashingMachineCode}", 
            washingMachine.Id, request.WashingMachineCode);
        
        return Result.Created;
    }
    
    private async Task<ErrorOr<Success>> IsValidAsync(CreateWashingMachineCommand request, CancellationToken cancellationToken)
    {
        if (request.LineQueues!.Length < 1 ||
            request.LineQueues.Length > 2 ||
            request.LineQueues.Any(x => string.IsNullOrWhiteSpace(x.WashingMachineLineCode)) ||
            request.LineQueues.DistinctBy(x => x.WashingMachineLineCode).Count() != request.LineQueues.Length)
        {
            logger.LogError("Washing machine lines are invalid. We cannot create new one in planning. Line codes: {LineCodes}",
                string.Join(", ", request.LineQueues.Select(x => x.WashingMachineLineCode)));
            return WashingMachineErrors.ValidationInvalidLineQueues;
        }
        
        var existingWashingMachine = await sender.Send(
            new GetWashingMachineByCodeQuery(request.WashingMachineCode), cancellationToken);

        if (existingWashingMachine.HasValue())
        {
            logger.LogWarning("Washing machine with code {WashingMachineCode} already exists in planning. We cannot create new one.", 
                request.WashingMachineCode);
            return WashingMachineErrors.ValidationWashingMachineAlreadyExists;
        }

        return Result.Success;
    }
}