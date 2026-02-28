namespace Nexticz.Module.EmailSender.Domain.EmailMessageAggregate.Events;

public record EmailMessageSentEvent(
    Guid Id, DateTimeOffset SentAt, EmailAttachment[] Attachments);