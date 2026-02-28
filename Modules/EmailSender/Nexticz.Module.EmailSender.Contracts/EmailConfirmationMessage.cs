namespace Nexticz.Module.EmailSender.Contracts;

public record EmailConfirmationMessage(
    Guid EmailId,
    string[] ToRecipients,
    string[] CcRecipients,
    string[] BccRecipients,
    int AttachmentsCount,
    string ModuleName, 
    string EmailType,
    Dictionary<string, string> InitiatorProperties,
    EmailStatusContract EmailStatus,
    DateTimeOffset ProcessedAt,
    string? FailureReason);