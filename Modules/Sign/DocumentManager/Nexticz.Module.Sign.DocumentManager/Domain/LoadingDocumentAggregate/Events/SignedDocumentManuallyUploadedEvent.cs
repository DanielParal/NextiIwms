using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

public record SignedDocumentManuallyUploadedEvent(
    Guid Id,
    string LoadingDocumentCode,
    string? DeliveryDocumentCode,
    bool IsLoadingDocumentSignature,
    DateTimeOffset UploadedAt,
    string UploadedByUserName,
    string? UploadedByUserFullName,
    string? DepositorName) : IMartenEvent;