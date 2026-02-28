using Marten.Events.Projections;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.SentEmails;

public class SentEmailProjection : MultiStreamProjection<SentEmailView, Guid>
{
    public SentEmailProjection()
    {
        Identity<LoadingDocumentEmailSentEvent>(x => GenerateProjectionId(x.EmailId));

        ProjectEvent<LoadingDocumentEmailSentEvent>((view, currentEvent) =>
        {
            view.EmailId = currentEvent.EmailId;
            view.Recipients = string.Join(',', currentEvent.Recipients);
            view.CreatedAt = currentEvent.ProcessedAt;
            view.Metadata  = currentEvent.Metadata;
            view.AttachmentsCount = currentEvent.AttachmentsCount;
            view.FailureReason = currentEvent.FailureReason;
        });
    }
    
    private static Guid GenerateProjectionId(Guid emailId)
    {
        var newIdWithPrefix = $"SentEmail-{emailId}";
        var bytes = System.Text.Encoding.UTF8.GetBytes(newIdWithPrefix);
        var hash = System.Security.Cryptography.MD5.HashData(bytes);
        return new Guid(hash);
    }
}