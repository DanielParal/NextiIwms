using MassTransit;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Module.Sign.Settings.Application.Users.Orchestrators;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Consumers;

internal class UserChangedNotificationConsumer(
    ILogger<UserChangedNotificationConsumer> logger,
    IUserChangedOrchestrator userChangedOrchestrator) 
    : IConsumer<UserChangedMessage>
{
    public async Task Consume(ConsumeContext<UserChangedMessage> context)
    {
        logger.LogTrace("SIGN - Settings - consumer {NotificationName} received notification. UserName: {UserName}, UserChangeType: {NotificationType}", 
            nameof(UserChangedNotificationConsumer), context.Message.Username, context.Message.UserChangeType);
        
        await userChangedOrchestrator.OrchestrateAsync(context.Message, context.CancellationToken);
    }
}