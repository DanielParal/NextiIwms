using ErrorOr;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;

namespace Nexticz.Module.EmailSender.Application.EmailMessages.Commands.SaveEmailMessageFailed;

internal record SaveEmailMessageFailedCommand(Guid Id, string ErrorMessage) : IEmailSenderCommand<ErrorOr<EmailMessage>>;