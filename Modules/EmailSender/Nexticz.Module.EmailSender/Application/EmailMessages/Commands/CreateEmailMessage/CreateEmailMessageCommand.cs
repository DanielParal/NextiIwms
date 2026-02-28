using ErrorOr;
using Nexticz.Module.EmailSender.Contracts;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;

namespace Nexticz.Module.EmailSender.Application.EmailMessages.Commands.CreateEmailMessage;

internal record CreateEmailMessageCommand(SendEmailMessage SendEmailMessage) : IEmailSenderCommand<ErrorOr<EmailMessage>>;