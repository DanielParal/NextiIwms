using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

public record LoadingDocumentPrintCopiesCountChangedEvent(
    Guid Id,
    string LoadingDocumentCode,
    string? DeliveryDocumentCode,
    int PrintCopiesCount,
    DateTimeOffset ChangedAt,
    string ChangedByUserName) : IMartenEvent;