namespace Nexticz.Module.EmailSender.Domain.EmailMessageAggregate.Events;

public record EmailMessageScheduledEvent(
    Guid Id, 
    string[] ToRecipients, 
    string[] CcRecipients, 
    string[] BccRecipients, 
    string Subject, 
    string TextBody,
    string HtmlBody,
    DateTimeOffset CreatedAt,
    DateTimeOffset ScheduledToBeSentAt,
    EmailInitiator Initiator,
    string? AttachmentsFolderGuid);