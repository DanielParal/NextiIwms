using MassTransit;
using Nexticz.Lib.Shared.MassTransit;
using Nexticz.Module.Mmo.Settings.Infrastructure.Consumers;

namespace Nexticz.Module.Mmo.Settings.Infrastructure;

public class MassTransitRegistrator : IConsumerRegistrar
{
    public void Register(IBusRegistrationConfigurator cfg)
    {
        cfg.AddConsumer<UserChangedNotificationConsumer, UserChangedNotificationConsumerDefinition>();
    }
}