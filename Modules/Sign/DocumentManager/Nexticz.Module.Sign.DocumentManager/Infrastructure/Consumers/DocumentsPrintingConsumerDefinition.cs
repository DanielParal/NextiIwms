using MassTransit;
using Nexticz.Lib.Shared.MassTransit;
using Nexticz.Module.Sign.DocumentManager.Contracts.Printings;
using Nexticz.Module.Sign.SharedKernel.ModuleConfiguration;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.Consumers;

internal class DocumentsPrintingConsumerDefinition : ConsumerDefinition<DocumentsPrintingConsumer>
{
    private static string QueuePrefix => ModuleNameProvider.MassTransitModulePrefixName;
    public static readonly int RetryCount = 12;
    
    public DocumentsPrintingConsumerDefinition()
    {
        EndpointName = $"{QueuePrefix}-{KebabCaseEndpointNameFormatter.Instance.Consumer<DocumentsPrintingConsumer>()}";
    }
    
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<DocumentsPrintingConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        const int partitionCount = 4;

        endpointConfigurator.PrefetchCount = 8;
        
        endpointConfigurator.UseMessageRetry(r =>
        {
            r.Handle<HandleLaterException>();
            r.Interval(RetryCount, TimeSpan.FromSeconds(5));
        });


        consumerConfigurator.Message<DocumentsPrintingRequested>(m =>
        {
            m.UsePartitioner(partitionCount, x => x.Message.PrinterCode);
        });
    }
}