namespace Nexticz.Module.EmailSender.Contracts;

public record SendEmailMessage(
    string[] ToRecipients, 
    string[] CcRecipients, 
    string[] BccRecipients, 
    string Subject, 
    string HtmlBody, 
    string TextBody, 
    EmailInitiatorContract Initiator,
    string? AttachmentsFolderGuid);