using MassTransit;
using Nexticz.Lib.Shared.Logging;

namespace Nexticz.Lib.Shared.MessagePublishers;

public class BaseMessagePublisher(IPublishEndpoint publishEndpoint) : IBaseMessagePublisher
{
    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        var correlationId = CorrelationIdProvider.Instance.GetInternalId();

        return publishEndpoint.Publish(message, ctx =>
        {
            ctx.Headers.Set(CorrelationIdProvider.CorrelationIdHeaderName, correlationId);
        }, cancellationToken);
    }
}