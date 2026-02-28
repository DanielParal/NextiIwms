using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Notifications.Contracts.SignalRNotifications;
using Nexticz.Module.Notifications.Application.SignalRNotifications.Commands.SendSignalRNotification;
using Nexticz.Module.Notifications.Domain;

namespace Nexticz.Module.Notifications.Infrastructure.Consumers;

internal class SignalrNotificationConsumer(
    ILogger<SignalrNotificationConsumer> logger,
    ISender sender) 
    : IConsumer<SignalRPublishedNotification>
{
    public async Task Consume(ConsumeContext<SignalRPublishedNotification> context)
    {
        logger.LogTrace("SignalR notification received. Module name: {ModuleName}, notification type: {NotificationType}, title: {MessageTitle}, " +
                              "number of receivers: {ReceiversCount}, contains data: {DoesContainData}", 
            context.Message.ModuleName, context.Message.NotificationType, context.Message.Title, context.Message.Receivers.Length, context.Message.Data != null);
        
        
        await sender.Send(new SendSignalRNotificationCommand(context.Message.Title, context.Message.Description, context.Message.Data, 
            context.Message.NotificationType, context.Message.ModuleName, (NotificationSeverity)context.Message.Severity, context.Message.Receivers));
    }
}