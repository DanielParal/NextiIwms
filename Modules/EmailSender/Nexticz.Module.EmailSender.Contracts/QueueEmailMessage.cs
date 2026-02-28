namespace Nexticz.Module.EmailSender.Contracts;

public record QueueEmailMessage(
    string[] ToRecipients, 
    string[] CcRecipients, 
    string[] BccRecipients, 
    string Subject, 
    string HtmlBody, 
    string TextBody,
    DateTimeOffset ScheduledToBeSentAt,
    EmailInitiatorContract Initiator,
    string? AttachmentsFolderGuid);