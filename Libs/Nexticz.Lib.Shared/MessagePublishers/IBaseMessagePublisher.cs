namespace Nexticz.Lib.Shared.MessagePublishers;

public interface IBaseMessagePublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : class;
}