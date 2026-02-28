using MassTransit;
using Nexticz.Lib.Shared.MassTransit;
using Nexticz.Module.Notifications.Infrastructure.Consumers;

namespace Nexticz.Module.Notifications.Infrastructure;

public class MassTransitRegistrator : IConsumerRegistrar
{
    public void Register(IBusRegistrationConfigurator cfg)
    {
        cfg.AddConsumer<SignalrNotificationConsumer, SignalrNotificationConsumerDefinition>();
    }
}