using MassTransit;
using MediatR;

namespace Nexticz.Module.Auth.Infrastructure.Messaging;

// public class UserRolesChangedConsumerDefinition : ConsumerDefinition<UserRolesChangedConsumer>
// {
//     public UserRolesChangedConsumerDefinition()
//     {
//         EndpointName = QueueingConfiguration.BagTrashTypeCodeUpdatedEndpointName;
//         ConcurrentMessageLimit = 1;
//     }
//     
//     protected override void ConfigureConsumer(
//         IReceiveEndpointConfigurator endpointConfigurator, 
//         IConsumerConfigurator<BagTrashTypeCodeUpdatedConsumer> consumerConfigurator,
//         IRegistrationContext context)
//     {
//         endpointConfigurator.UseMessageRetry(r => r.None());
//     }
// }
//
// public class UserRolesChangedConsumer : IConsumer<BagTrashTypeCodeUpdated>
// {
//     private readonly IPublisher _mediatorPublisher;
//
//     public UserRolesChangedConsumer(IPublisher mediatorPublisher)
//     {
//         _mediatorPublisher = mediatorPublisher;
//     }
//
//     public async Task Consume(ConsumeContext<BagTrashTypeCodeUpdated> context)
//     {
//         await _mediatorPublisher.Publish(context.Message, context.CancellationToken);
//     }
// }