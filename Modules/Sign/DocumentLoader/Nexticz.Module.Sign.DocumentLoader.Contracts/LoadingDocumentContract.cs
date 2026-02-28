namespace Nexticz.Module.Sign.DocumentLoader.Contracts;

public record LoadingDocumentContract(
    string Code,
    int? GateNumber,
    string DepositorCode,
    string DeliveryMethodCode,
    string? LicensePlate,
    string? DriverName,
    decimal? Weight,
    int? AdrPoints,
    string? LoadingLocation,
    string LoadingInWmsFinishedBy,
    DateTime LoadingInWmsFinishedAt,
    DeliveryDocumentContract[] DeliveryNotes);