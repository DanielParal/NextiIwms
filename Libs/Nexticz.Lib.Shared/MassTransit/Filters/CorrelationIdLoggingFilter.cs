using MassTransit;
using Nexticz.Lib.Shared.Generators;
using Nexticz.Lib.Shared.Logging;

namespace Nexticz.Lib.Shared.MassTransit.Filters;

internal class CorrelationIdLoggingFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private const string FilterName = "CorrelationIdLoggingFilter";
    
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var sourceCorrelationId = context.Headers.Get<string>(CorrelationIdProvider.CorrelationIdHeaderName) ?? string.Empty;
        var threadId = Base62IdGenerator.GenerateId(3);
        CorrelationIdProvider.Instance.SetExternalIdAsPrefix($"{sourceCorrelationId}-{threadId}");

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope(FilterName);
    }
}