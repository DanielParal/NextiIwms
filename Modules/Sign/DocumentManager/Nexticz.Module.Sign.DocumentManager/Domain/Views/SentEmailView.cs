namespace Nexticz.Module.Sign.DocumentManager.Domain.Views;

public class SentEmailView
{
    public Guid Id { get; set; }
    public Guid EmailId { get; set; }
    public string Recipients { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string Metadata { get; set; }
    public int AttachmentsCount { get; set; }
    public string? FailureReason { get; set; }
}