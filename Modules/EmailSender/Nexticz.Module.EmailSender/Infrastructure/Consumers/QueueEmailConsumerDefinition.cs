using MassTransit;

namespace Nexticz.Module.EmailSender.Infrastructure.Consumers;

internal class QueueEmailConsumerDefinition : ConsumerDefinition<QueueEmailConsumer>
{
    private static string QueuePrefix => MassTransitModulePrefixName.ModulePrefixName;
    
    public QueueEmailConsumerDefinition()
    {
        EndpointName = $"{QueuePrefix}-{KebabCaseEndpointNameFormatter.Instance.Consumer<QueueEmailConsumer>()}";
    }
}