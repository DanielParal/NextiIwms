using MassTransit;

namespace Nexticz.Module.Notifications.Infrastructure.Consumers;

internal class SignalrNotificationConsumerDefinition : ConsumerDefinition<SignalrNotificationConsumer>
{
    private static string QueuePrefix => MassTransitModulePrefixName.ModulePrefixName;
    
    public SignalrNotificationConsumerDefinition()
    {
        EndpointName = $"{QueuePrefix}-{KebabCaseEndpointNameFormatter.Instance.Consumer<SignalrNotificationConsumer>()}";
    }
}