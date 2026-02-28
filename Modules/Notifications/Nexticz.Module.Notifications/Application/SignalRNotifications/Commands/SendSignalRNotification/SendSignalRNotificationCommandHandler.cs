using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Notifications.Contracts.OpenApiContracts;
using Nexticz.Module.Notifications.Contracts.SignalRNotifications;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Notifications.Application.Interfaces;
using Nexticz.Module.Notifications.Domain.SignalRNotificationAggregate;
using Nexticz.Module.Notifications.Domain.SignalRNotificationAggregate.Events;

namespace Nexticz.Module.Notifications.Application.SignalRNotifications.Commands.SendSignalRNotification;

internal class SendSignalRNotificationCommandHandler(
    INotificationUnitOfWork unitOfWork,
    ISignalRNotifier signalRNotifier,
    ILogger<SendSignalRNotificationCommandHandler> logger,
    IClock clock)
    : IRequestHandler<SendSignalRNotificationCommand, ErrorOr<Success>>
{
    private const string SignalRReceiveMethodName = nameof(SignalRReceiveNotificationNameContract.ReceiveNotification);
    public async Task<ErrorOr<Success>> Handle(SendSignalRNotificationCommand request, CancellationToken cancellationToken)
    {
        var validationResult = Validate(request);
        if (!validationResult.IsSuccess)
        {
            var failedSignalR = new SignalRNotification(request.Title, request.Message, request.Data, request.NotificationType, request.ModuleName, request.Severity, request.Receivers);
            var failedSignalREvent = new SignalRNotificationFailedEvent(failedSignalR.Id, failedSignalR.Title, failedSignalR.Message, 
                failedSignalR.Data, failedSignalR.NotificationType, failedSignalR.ModuleName, request.Severity, failedSignalR.Receivers, validationResult.FailureReasons);
            unitOfWork.StartStream<SignalRNotificationFailedEvent, SignalRNotification>(failedSignalR.Id, failedSignalREvent);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            logger.LogWarning("Notification - SignalR notification failed. SignalR Id: {SignalRId}, notificationType: {NotificationType}, moduleName: {ModuleName}, title: {Title}. Failed reasons: {FailedReasons}.", 
                failedSignalR.Id, failedSignalR.NotificationType, failedSignalR.ModuleName, failedSignalR.Title, string.Join(", ", validationResult.FailureReasons));
            return SignalRNotificationErrors.ValidationSignalRFailedToSend;
        }
        
        var signalR = new SignalRNotification(request.Title, request.Message, request.Data, request.NotificationType, request.ModuleName, request.Severity, request.Receivers);

        await signalRNotifier.BroadcastNotificationAsync(signalR, cancellationToken);
        
        var signalREvent = new SignalRNotificationSentEvent(signalR.Id, signalR.Title, signalR.Message, signalR.Data, signalR.NotificationType, signalR.ModuleName, request.Severity, signalR.Receivers);
        unitOfWork.StartStream<SignalRNotificationSentEvent, SignalRNotification>(signalR.Id, signalREvent);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Notification - SignalR notification sent. SignalR Id: {SignalRId}, notificationType: {NotificationType}, moduleName: {ModuleName}, title: {Title}", 
            signalR.Id, signalR.NotificationType, signalR.ModuleName, signalR.Title);
        
        return Result.Success;
    }
    
    private static (bool IsSuccess, string[] FailureReasons) Validate(SendSignalRNotificationCommand request)
    {
        var isSuccess = true;
        var failedReasons = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            isSuccess = false;
            failedReasons.Add("Title is null or empty.");
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            isSuccess = false;
            failedReasons.Add("Message is null or empty.");
        }

        if (request.Receivers.Length == 0)
        {
            isSuccess = false;
            failedReasons.Add("There are no receivers.");
        }

        if (string.IsNullOrWhiteSpace(request.ModuleName))
        {
            isSuccess = false;
            failedReasons.Add("Module name is null or empty.");
        }

        if (string.IsNullOrWhiteSpace(request.NotificationType))
        {
            isSuccess = false;
            failedReasons.Add("Notification type is null or empty.");
        }
        
        return (isSuccess, failedReasons.ToArray());
    }
}