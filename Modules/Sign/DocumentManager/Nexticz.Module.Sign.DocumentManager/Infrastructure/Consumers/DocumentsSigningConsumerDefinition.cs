using MassTransit;
using Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;
using Nexticz.Module.Sign.SharedKernel.ModuleConfiguration;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.Consumers;

internal class DocumentsSigningConsumerDefinition : ConsumerDefinition<DocumentsSigningConsumer>
{
    private static string QueuePrefix => ModuleNameProvider.MassTransitModulePrefixName;
    
    public DocumentsSigningConsumerDefinition()
    {
        EndpointName = $"{QueuePrefix}-{KebabCaseEndpointNameFormatter.Instance.Consumer<DocumentsSigningConsumer>()}";
    }
    
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<DocumentsSigningConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        const int partitionCount = 4;

        endpointConfigurator.PrefetchCount = 8;

        consumerConfigurator.Message<SignDocumentsRequested>(m =>
        {
            m.UsePartitioner(partitionCount, x => x.Message.SigningDeviceCode);
        });
    }
}