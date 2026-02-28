namespace Nexticz.Module.EmailSender.Domain.EmailMessageAggregate.Events;

public record EmailMessageCreatedEvent(
    Guid Id, 
    string[] ToRecipients, 
    string[] CcRecipients, 
    string[] BccRecipients, 
    string Subject, 
    string TextBody,
    string HtmlBody,
    DateTimeOffset CreatedAt,
    EmailInitiator Initiator,
    string? AttachmentsFolderGuid);