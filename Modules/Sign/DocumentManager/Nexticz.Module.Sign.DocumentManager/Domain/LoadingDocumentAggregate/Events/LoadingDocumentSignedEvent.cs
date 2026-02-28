using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

public record LoadingDocumentSignedEvent(
    Guid Id,
    string Code,
    bool IsLoadingDocumentAlsoSigned,
    string[] DeliveryDocumentCodes,
    DateTimeOffset SignedAt,
    string SignedByUserName,
    string SignedByFullName,
    string DriverName,
    string LicensePlate,
    string? DepositorName) : IMartenEvent;