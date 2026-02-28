using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

public record LoadingDocumentEmailSentEvent(Guid Id, string LoadingDocumentCode, string? DeliveryDocumentCode, Guid EmailId, 
    string[] Recipients, DateTimeOffset ProcessedAt, string Metadata, int AttachmentsCount, string? FailureReason) : IMartenEvent;