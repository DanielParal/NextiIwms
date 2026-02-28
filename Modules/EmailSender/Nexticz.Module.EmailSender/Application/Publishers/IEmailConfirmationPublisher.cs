using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;

namespace Nexticz.Module.EmailSender.Application.Publishers;

internal interface IEmailConfirmationPublisher
{
    Task PublishEmailConfirmationAsync(Guid emailId, string[] toRecipients, string[] ccRecipients, string[] bccRecipients, int attachmentsCount, EmailInitiator initiator, string? errorMessage, CancellationToken cancellationToken);
}