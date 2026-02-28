namespace Nexticz.Module.EmailSender.Publishers;

public record PublishEmailMessage(
    string[] ToRecipients, 
    string[] CcRecipients, 
    string[] BccRecipients,
    string Subject, 
    string HtmlBody, 
    string TextBody, 
    string ModuleName, 
    string EmailType, 
    bool ShouldSendConfirmationMessage,
    Dictionary<string, string> InitiatorProperties,
    PublishEmailAttachment[] Attachments,
    DateTimeOffset? ScheduledToBeSentAt);