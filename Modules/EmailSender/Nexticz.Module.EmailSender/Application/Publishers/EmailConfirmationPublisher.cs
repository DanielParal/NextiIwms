using MassTransit;
using Nexticz.Module.EmailSender.Contracts;
using Nexticz.Lib.Shared.MessagePublishers;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;

namespace Nexticz.Module.EmailSender.Application.Publishers;

internal class EmailConfirmationPublisher(
    IPublishEndpoint publishEndpoint,
    IClock clock) 
    : BaseMessagePublisher(publishEndpoint), IEmailConfirmationPublisher
{
    public async Task PublishEmailConfirmationAsync(Guid emailId, string[] toRecipients, string[] ccRecipients, string[] bccRecipients, int attachmentsCount, EmailInitiator initiator, string? errorMessage, CancellationToken cancellationToken)
    {
        var emailConfirmationMessage = 
            new EmailConfirmationMessage(
                emailId,
                toRecipients,
                ccRecipients, 
                bccRecipients,
                attachmentsCount,
                initiator.ModuleName,
                initiator.EmailType,
                initiator.InitiatorProperties,
                string.IsNullOrWhiteSpace(errorMessage) ? EmailStatusContract.Sent : EmailStatusContract.Failed,
                clock.UtcNowOffset,
                errorMessage);

        await PublishAsync(emailConfirmationMessage, cancellationToken);
    }
}