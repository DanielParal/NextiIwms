using MassTransit;
using Nexticz.Module.Sign.SharedKernel.ModuleConfiguration;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.Consumers;

internal class EmailConfirmationConsumerDefinition : ConsumerDefinition<EmailConfirmationConsumer>
{
    private static string QueuePrefix => ModuleNameProvider.MassTransitModulePrefixName;
    
    public EmailConfirmationConsumerDefinition()
    {
        EndpointName = $"{QueuePrefix}-{KebabCaseEndpointNameFormatter.Instance.Consumer<EmailConfirmationConsumer>()}";
    }
}