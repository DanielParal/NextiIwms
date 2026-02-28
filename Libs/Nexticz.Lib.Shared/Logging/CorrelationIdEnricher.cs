using Serilog.Core;
using Serilog.Events;

namespace Nexticz.Lib.Shared.Logging;

internal class CorrelationIdEnricher : ILogEventEnricher
{
    private static readonly string CorrelationIdPropertyName = "CorrelationId";

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var correlationId = CorrelationIdProvider.Instance.GetInternalId();

        var correlationIdProperty = propertyFactory.CreateProperty(CorrelationIdPropertyName, correlationId);
        logEvent.AddOrUpdateProperty(correlationIdProperty);
    }
}