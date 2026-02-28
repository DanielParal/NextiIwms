using MassTransit;
using Nexticz.Lib.Shared.MassTransit;
using Nexticz.Module.Sign.DocumentManager.Infrastructure.Consumers;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure;

public class MassTransitRegistrator : IConsumerRegistrar
{
    public void Register(IBusRegistrationConfigurator cfg)
    {
        cfg.AddConsumer<EmailConfirmationConsumer, EmailConfirmationConsumerDefinition>();
        cfg.AddConsumer<DocumentsPrintingConsumer, DocumentsPrintingConsumerDefinition>();
        cfg.AddConsumer<DocumentsSigningConsumer, DocumentsSigningConsumerDefinition>();
    }
}