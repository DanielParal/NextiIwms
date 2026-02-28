using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

public record LoadingDocumentCreatedEvent(
    Guid Id,
    string Code,
    int? GateNumber,
    string DepositorCode,
    string? DepositorName,
    string DeliveryMethodCode,
    string? DeliveryMethodName,
    string? LicensePlate,
    string? DriverName,
    decimal? Weight,
    int? AdrPoints,
    string? LoadingLocation,
    string LoadingInWmsFinishedBy,
    DateTimeOffset LoadingInWmsFinishedAt,
    DateTimeOffset CreatedAt,
    int RequestedPrintCopiesCount,
    DeliveryDocument[] DeliveryDocuments
) : IMartenEvent;