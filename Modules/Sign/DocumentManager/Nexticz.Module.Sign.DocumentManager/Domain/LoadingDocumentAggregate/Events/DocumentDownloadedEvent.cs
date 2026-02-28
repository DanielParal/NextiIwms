using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

public record DocumentDownloadedEvent(
    Guid Id,
    string LoadingDocumentCode,
    string? DeliveryDocumentCode,
    DateTimeOffset DownloadedAt,
    string DownloadedByUserName) : IMartenEvent;