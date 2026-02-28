
namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

public class SentEmail
{
    public Guid EmailId { get; private set; }
    public string[] Recipients { get; private set; }
    public DateTimeOffset ProcessedAt { get; private set; }
    public int AttachmentsCount { get; private set; }
    public string? FailureReason { get; set; }
    
    public SentEmail(
        Guid emailId,
        string[] recipients,
        DateTimeOffset processedAt,
        int attachmentsCount,
        string? failureReason)
    {
        EmailId = emailId;
        Recipients = recipients;
        ProcessedAt = processedAt;
        AttachmentsCount = attachmentsCount;       
        FailureReason = failureReason;       
    }
}