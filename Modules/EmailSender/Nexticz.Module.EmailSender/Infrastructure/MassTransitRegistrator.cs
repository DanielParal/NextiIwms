using MassTransit;
using Nexticz.Lib.Shared.MassTransit;
using Nexticz.Module.EmailSender.Infrastructure.Consumers;

namespace Nexticz.Module.EmailSender.Infrastructure;

public class MassTransitRegistrator : IConsumerRegistrar
{
    public void Register(IBusRegistrationConfigurator cfg)
    {
        cfg.AddConsumer<SendEmailConsumer, SendEmailConsumerDefinition>();
        cfg.AddConsumer<QueueEmailConsumer, QueueEmailConsumerDefinition>();
    }
}