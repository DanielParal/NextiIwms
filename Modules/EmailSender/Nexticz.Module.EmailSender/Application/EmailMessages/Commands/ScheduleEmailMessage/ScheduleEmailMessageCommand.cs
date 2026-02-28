using ErrorOr;
using Nexticz.Module.EmailSender.Contracts;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;

namespace Nexticz.Module.EmailSender.Application.EmailMessages.Commands.ScheduleEmailMessage;

internal record ScheduleEmailMessageCommand(QueueEmailMessage QueueEmailMessage) : IEmailSenderCommand<ErrorOr<EmailMessage>>;