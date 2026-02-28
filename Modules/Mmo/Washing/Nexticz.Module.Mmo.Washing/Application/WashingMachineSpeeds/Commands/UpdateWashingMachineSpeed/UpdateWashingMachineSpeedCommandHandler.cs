using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Module.Mmo.Washing.Contracts.WashingMachineSpeeds.Notifications;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Commands.UpdateWashingMachineSpeed;

internal class UpdateWashingMachineSpeedCommandHandler(
    ILogger<UpdateWashingMachineSpeedCommandHandler> logger,
    IClock clock,
    IWashingUnitOfWork washingUnitOfWork,
    IWashingNotificationCollector notificationCollector) : IRequestHandler<UpdateWashingMachineSpeedCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> Handle(UpdateWashingMachineSpeedCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            logger.LogError("Washing - washing machine code is required. We cannot update washing machine speed. Id: {Id}, Speed: {Speed}",
                request.Id, request.Speed);
            return Task.FromResult<ErrorOr<Success>>(WashingMachineSpeedErrors.ValidationCodeIsRequired);
        }
        
        if (request.Speed < 0)
        {
            logger.LogError("Washing - washing machine speed must not be negative. We cannot update washing machine speed. Id: {Id}, Speed: {Speed}",
                request.Id, request.Speed);
            return Task.FromResult<ErrorOr<Success>>(WashingMachineSpeedErrors.ValidationSpeedMustNotBeNegative);
        }
        
        var dateChanged = clock.UtcNowOffset;
        var washingMachineSpeedUpdatedEvent = new WashingMachineSpeedUpdatedEvent(request.Id, request.Code, request.Speed, request.SpeedLevel, dateChanged);
        washingUnitOfWork.AppendEvent(request.Id, washingMachineSpeedUpdatedEvent);
        
        logger.LogInformation("Washing - washing machine speed updated. Id: {Id}, Code: {Code}, Speed: {Speed}.",
            request.Id, request.Code, request.Speed);

        notificationCollector.AddNotification(new WashingMachineSpeedChangedNotification(request.Code, request.Speed, (SpeedLevelContract)request.SpeedLevel, dateChanged));
        
        return Task.FromResult<ErrorOr<Success>>(Result.Success);
    }
}