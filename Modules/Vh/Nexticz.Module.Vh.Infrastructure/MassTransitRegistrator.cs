using MassTransit;
using Nexticz.Lib.Shared.MassTransit;
using Nexticz.Module.Vh.Infrastructure.Consumers;

namespace Nexticz.Module.Vh.Infrastructure;

public class MassTransitRegistrator : IConsumerRegistrar
{
    public void Register(IBusRegistrationConfigurator cfg)
    {
        cfg.AddConsumer<UserChangedNotificationConsumer, UserChangedNotificationConsumerDefinition>();
    }
}