namespace Nexticz.Module.EmailSender.Publishers;

public interface IEmailPublisher
{
    Task PublishSendEmailAsync(PublishEmailMessage message, CancellationToken cancellationToken);
}