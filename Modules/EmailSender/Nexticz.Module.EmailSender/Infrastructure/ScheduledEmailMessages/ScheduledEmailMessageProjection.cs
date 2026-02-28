using Marten.Events.Projections;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate.Events;
using Nexticz.Module.EmailSender.Domain.Views;

namespace Nexticz.Module.EmailSender.Infrastructure.ScheduledEmailMessages;

public class ScheduledEmailMessageProjection : MultiStreamProjection<ScheduledEmailMessageView, Guid>
{
    public ScheduledEmailMessageProjection()
    {
        Identity<EmailMessageScheduledEvent>(x => GenerateProjectionId(x.Id));
        Identity<EmailMessageSentEvent>(x => GenerateProjectionId(x.Id));
        Identity<EmailMessageFailedEvent>(x => GenerateProjectionId(x.Id));
        
        DeleteEvent<EmailMessageSentEvent>();
        DeleteEvent<EmailMessageFailedEvent>();
        
        ProjectEvent<EmailMessageScheduledEvent>((view, currentEvent) =>
        {
            view.EmailId = currentEvent.Id;
            view.ToRecipients = currentEvent.ToRecipients;
            view.CcRecipients = currentEvent.CcRecipients;
            view.BccRecipients = currentEvent.BccRecipients;
            view.Subject = currentEvent.Subject;
            view.TextBody = currentEvent.TextBody;
            view.HtmlBody = currentEvent.HtmlBody;
            view.ScheduledToBeSentAt = currentEvent.ScheduledToBeSentAt;
            view.Initiator = currentEvent.Initiator;
            view.AttachmentsFolderGuid = currentEvent.AttachmentsFolderGuid;
        });
    }
    
    private static Guid GenerateProjectionId(Guid emailId)
    {
        var newIdWithPrefix = $"ScheduledEmailMessage-{emailId}";
        var bytes = System.Text.Encoding.UTF8.GetBytes(newIdWithPrefix);
        var hash = System.Security.Cryptography.MD5.HashData(bytes);
        return new Guid(hash);
    }
}