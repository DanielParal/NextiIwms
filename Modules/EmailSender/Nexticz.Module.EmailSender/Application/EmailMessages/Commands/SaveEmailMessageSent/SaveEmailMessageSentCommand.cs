using ErrorOr;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;

namespace Nexticz.Module.EmailSender.Application.EmailMessages.Commands.SaveEmailMessageSent;

internal record SaveEmailMessageSentCommand(Guid Id, EmailAttachment[] Attachments) : IEmailSenderCommand<ErrorOr<EmailMessage>>;