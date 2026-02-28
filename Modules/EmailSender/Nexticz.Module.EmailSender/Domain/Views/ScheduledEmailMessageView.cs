using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;

namespace Nexticz.Module.EmailSender.Domain.Views;

public class ScheduledEmailMessageView
{
    public Guid Id { get; set; }
    public Guid EmailId { get; set; }
    public string[] ToRecipients { get; set; }
    public string[] CcRecipients { get; set; }
    public string[] BccRecipients { get; set; }
    public string Subject { get; set; }
    public string TextBody { get; set; }
    public string HtmlBody { get; set; }
    public EmailInitiator Initiator { get; set; }
    public string? AttachmentsFolderGuid { get; set; }
    public DateTimeOffset ScheduledToBeSentAt { get; set; }
}