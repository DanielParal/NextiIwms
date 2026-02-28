using ErrorOr;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.AddSentEmail;

internal record AddSentEmailCommand(
    string LoadingDocumentCode,
    string? DeliveryDocumentCode,
    Guid EmailId,
    int AttachmentsCount,
    string[] Recipients,
    DateTimeOffset ProcessedAt,
    string? FailureReason) : IDocumentManagerCommand<ErrorOr<Success>>;