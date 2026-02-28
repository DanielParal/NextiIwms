using MassTransit;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Vh.Application.VhUsers.Orchestrators;
using Nexticz.Module.Auth.Contracts.Accounts;

namespace Nexticz.Module.Vh.Infrastructure.Consumers;

internal class UserChangedNotificationConsumer(
    ILogger<UserChangedNotificationConsumer> logger,
    IUserChangedOrchestrator userChangedOrchestrator) 
    : IConsumer<UserChangedMessage>
{
    public async Task Consume(ConsumeContext<UserChangedMessage> context)
    {
        logger.LogTrace("VH - consumer {NotificationName} received notification. UserName: {UserName}, UserChangeType: {NotificationType}", 
            nameof(UserChangedNotificationConsumer), context.Message.Username, context.Message.UserChangeType);
        
        await userChangedOrchestrator.OrchestrateAsync(context.Message, context.CancellationToken);
    }
}