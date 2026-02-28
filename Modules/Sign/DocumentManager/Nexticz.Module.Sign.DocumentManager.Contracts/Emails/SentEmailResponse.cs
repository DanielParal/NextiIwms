using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.Emails;

public record SentEmailResponse(
    [property: Required] Guid Id,
    [property: Required] string Recipients,
    [property: Required] string Metadata,
    [property: Required] int AttachmentsCount,
    string? FailureReason,
    [property: Required] DateTimeOffset CreatedAt);