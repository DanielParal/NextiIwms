using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Module.Mmo.Washing.Contracts.WashingMachineSpeeds.Notifications;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Commands.CreateWashingMachineSpeed;

internal class CreateWashingMachineSpeedCommandHandler(
    ILogger<CreateWashingMachineSpeedCommandHandler> logger,
    IClock clock,
    IWashingUnitOfWork washingUnitOfWork,
    IWashingNotificationCollector notificationCollector) : IRequestHandler<CreateWashingMachineSpeedCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(CreateWashingMachineSpeedCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            logger.LogError("Washing - washing machine code is required. We cannot create washing machine speed.Speed: {Speed}",
                request.Speed);
            return WashingMachineSpeedErrors.ValidationCodeIsRequired;
        }
        
        if (request.Speed < 0)
        {
            logger.LogError("Washing - washing machine speed must not be negative. We cannot create washing machine speed. Code: {Code}, Speed: {Speed}",
                request.Code, request.Speed);
            return WashingMachineSpeedErrors.ValidationSpeedMustNotBeNegative;
        }
        
        var dateChanged = clock.UtcNowOffset;
        var washingMachineSpeed = new WashingMachineSpeed(request.Code, request.Speed, request.SpeedLevel);
        var washingMachineSpeedCreatedEvent = new WashingMachineSpeedCreatedEvent(washingMachineSpeed.Id, washingMachineSpeed.Code, washingMachineSpeed.Speed, washingMachineSpeed.SpeedLevel, dateChanged);
        
        washingUnitOfWork.StartStream<WashingMachineSpeedCreatedEvent, WashingMachineSpeed>(washingMachineSpeed.Id, washingMachineSpeedCreatedEvent);
        
        logger.LogInformation("Washing - washing machine speed created. Id: {Id}, Code: {Code}, Speed: {Speed}.",
            washingMachineSpeed.Id, washingMachineSpeed.Code, washingMachineSpeed.Speed);

        notificationCollector.AddNotification(new WashingMachineSpeedChangedNotification(washingMachineSpeed.Code, washingMachineSpeed.Speed, (SpeedLevelContract)washingMachineSpeed.SpeedLevel, dateChanged));
        
        return Result.Success;
    }
}