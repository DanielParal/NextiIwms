using MassTransit;

namespace Nexticz.Module.Vh.Infrastructure.Consumers;

internal class UserChangedNotificationConsumerDefinition : ConsumerDefinition<UserChangedNotificationConsumer>
{
    private static string QueuePrefix => MassTransitModulePrefixName.ModulePrefixName;
    
    public UserChangedNotificationConsumerDefinition()
    {
        EndpointName = $"{QueuePrefix}-{KebabCaseEndpointNameFormatter.Instance.Consumer<UserChangedNotificationConsumer>()}";
    }
}