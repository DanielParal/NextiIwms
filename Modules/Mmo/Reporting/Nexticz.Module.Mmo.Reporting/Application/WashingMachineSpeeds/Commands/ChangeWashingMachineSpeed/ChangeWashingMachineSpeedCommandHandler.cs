using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.WashingMachineSpeeds.Queries.GetCurrentWashingMachineSpeedByCode;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Application.WashingMachineSpeeds.Commands.ChangeWashingMachineSpeed;

internal class ChangeWashingMachineSpeedCommandHandler(
    ILogger<ChangeWashingMachineSpeedCommandHandler> logger,
    IReportingUnitOfWork unitOfWork,
    ISender sender) 
    : IRequestHandler<ChangeWashingMachineSpeedCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ChangeWashingMachineSpeedCommand request, CancellationToken cancellationToken)
    {
        var currentWashingMachineSpeed = 
            await sender.Send(new GetCurrentWashingMachineSpeedByCodeQuery(request.Code), cancellationToken);

        if (!currentWashingMachineSpeed.IsError)
        {
            var speedEndedEvent =
                new WashingMachineSpeedEndedEvent(currentWashingMachineSpeed.Value.Id, request.DateChanged);
            unitOfWork.AppendEvent(currentWashingMachineSpeed.Value.Id, speedEndedEvent);
            
            logger.LogInformation("Reporting - washing machine speed ended. Id: {Id}, Code: {Code}, DateEnded: {DateEnded}.",
                currentWashingMachineSpeed.Value.Id, currentWashingMachineSpeed.Value.Code, request.DateChanged);
        }
        
        var washingMachineSpeed = new WashingMachineSpeed(request.Code, request.Speed, request.SpeedLevel, request.DateChanged);
        var speedStartedEvent =
            new WashingMachineSpeedStartedEvent(
                washingMachineSpeed.Id, washingMachineSpeed.Code, washingMachineSpeed.Speed, washingMachineSpeed.SpeedLevel, washingMachineSpeed.DateStarted);
        unitOfWork.AppendEvent(washingMachineSpeed.Id, speedStartedEvent);
        
        logger.LogInformation("Reporting - washing machine speed started. Id: {Id}, Code: {Code}, Speed: {Speed}, SpeedLevel: {SpeedLevel}, DateStarted: {DateStarted}.",
            washingMachineSpeed.Id, washingMachineSpeed.Code, washingMachineSpeed.Speed, washingMachineSpeed.SpeedLevel, washingMachineSpeed.DateStarted);
        
        return Result.Success;
    }
}