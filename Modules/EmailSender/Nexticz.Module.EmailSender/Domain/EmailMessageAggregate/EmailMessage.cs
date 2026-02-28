using ErrorOr;
using JasperFx.Core;
using Nexticz.Lib.Shared.DomainCore;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate.Events;

namespace Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;

public class EmailMessage : AggregateRoot
{
    public string[] ToRecipients { get; private set; }
    public string[] CcRecipients { get; private set; }
    public string[] BccRecipients { get; private set; }
    public string Subject { get; private set; }
    public string TextBody { get; private set; }
    public string HtmlBody { get; private set; }
    public EmailInitiator Initiator { get; private set; }
    public string? AttachmentsFolderGuid { get; private set; }
    public EmailAttachment[] Attachments { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? SentAt { get; private set; }
    public DateTimeOffset? ScheduledToBeSentAt { get; private set; }
    public DateTimeOffset? FailedAt { get; private set; }
    public string? ErrorMessage { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private EmailMessage() {}
    
    private EmailMessage(string[] toRecipients, string[] ccRecipients, string[] bccRecipients, 
        string subject, string textBody, string htmlBody, DateTimeOffset createdAt,
        EmailInitiator initiator, string? attachmentsFolderGuid = null, EmailAttachment[]? attachments = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        ToRecipients = toRecipients;
        CcRecipients = ccRecipients;
        BccRecipients = bccRecipients;
        Subject = subject;
        TextBody = textBody;
        HtmlBody = htmlBody;
        Initiator = initiator;
        AttachmentsFolderGuid = attachmentsFolderGuid;
        Attachments = attachments ?? [];
        CreatedAt = createdAt;
        SentAt = null;
        ScheduledToBeSentAt = null;
        FailedAt = null;
        ErrorMessage = null;
    }

    public static ErrorOr<EmailMessage> CreateNew(string[] toRecipients, string[] ccRecipients, string[] bccRecipients, string subject, string textBody, string htmlBody, DateTimeOffset createdAt,
        EmailInitiator initiator, string? attachmentsFolderGuid = null, EmailAttachment[]? attachments = null)
    {
        if (toRecipients.Length == 0 && ccRecipients.Length == 0 && bccRecipients.Length == 0)
            return EmailMessageDomainErrors.ValidationAtLeastOneEmailIsRequired;

        if (initiator.ShouldSendConfirmationMessage &&
            (string.IsNullOrWhiteSpace(initiator.ModuleName) ||
             string.IsNullOrWhiteSpace(initiator.EmailType)))
        {
            return EmailMessageDomainErrors.ValidationInitiatorIsNotCorrectlySet;
        }
        
        return new EmailMessage(toRecipients, ccRecipients, bccRecipients, subject, textBody, htmlBody, createdAt, initiator, attachmentsFolderGuid, attachments ?? []);
    }

    public void SendEmail(IClock clock, EmailAttachment[] attachments)
    {
        SentAt = clock.UtcNowOffset;
        Attachments = attachments; 
    }

    public void FailEmail(string errorMessage, IClock clock)
    {
        FailedAt = clock.UtcNowOffset;
        ErrorMessage = errorMessage;   
    }
    
    public void ScheduleEmail(DateTimeOffset scheduledToBeSentAt)
    {
        ScheduledToBeSentAt = scheduledToBeSentAt;  
    }

    public void Apply(EmailMessageCreatedEvent @event)
    {
        ToRecipients = @event.ToRecipients;
        CcRecipients = @event.CcRecipients;
        BccRecipients = @event.BccRecipients;
        Subject = @event.Subject;
        TextBody = @event.TextBody;
        HtmlBody = @event.HtmlBody;
        Initiator = @event.Initiator;
        AttachmentsFolderGuid = @event.AttachmentsFolderGuid;
        CreatedAt = @event.CreatedAt;

        Attachments = [];
        SentAt = null;
        ScheduledToBeSentAt = null;
        FailedAt = null;
        ErrorMessage = null;
    }
    
    public void Apply(EmailMessageScheduledEvent @event)
    {
        ToRecipients = @event.ToRecipients;
        CcRecipients = @event.CcRecipients;
        BccRecipients = @event.BccRecipients;
        Subject = @event.Subject;
        TextBody = @event.TextBody;
        HtmlBody = @event.HtmlBody;
        Initiator = @event.Initiator;
        AttachmentsFolderGuid = @event.AttachmentsFolderGuid;
        CreatedAt = @event.CreatedAt;
        ScheduledToBeSentAt = @event.ScheduledToBeSentAt;
        
        SentAt = null;
        FailedAt = null;
        ErrorMessage = null;
    }
    
    public void Apply(EmailMessageSentEvent @event)
    {
        SentAt = @event.SentAt;
        Attachments = @event.Attachments;
    }
    
    public void Apply(EmailMessageFailedEvent @event)
    {
        FailedAt = @event.FailedAt;
        ErrorMessage = @event.ErrorMessage;
    }
}