using MassTransit;

namespace Nexticz.Module.EmailSender.Infrastructure.Consumers;

internal class SendEmailConsumerDefinition : ConsumerDefinition<SendEmailConsumer>
{
    private static string QueuePrefix => MassTransitModulePrefixName.ModulePrefixName;
    
    public SendEmailConsumerDefinition()
    {
        EndpointName = $"{QueuePrefix}-{KebabCaseEndpointNameFormatter.Instance.Consumer<SendEmailConsumer>()}";
    }
}