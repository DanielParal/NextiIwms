using System.ComponentModel.DataAnnotations;
using Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.UnsignedLoadingDocuments;

public record UnsignedLoadingDocumentResponse(
    [property: Required] string Code,
    int? GateNumber,
    [property: Required] string DepositorCode,
    [property: Required] string DeliveryMethodCode,
    string? DeliveryMethodName,
    string? LicensePlate,
    string? DriverName,
    decimal? Weight,
    int? AdrPoints,
    string? LoadingLocation,
    [property: Required] string LoadingInWmsFinishedBy,
    [property: Required] DateTimeOffset LoadingInWmsFinishedAt,
    [property: Required] DateTimeOffset CreatedAt,
    [property: Required] int RequestedPrintCopiesCount,
    string? SigningDeviceCode,
    string? SentToSigningDeviceByUserName,
    DateTimeOffset? SentToSigningDeviceAt,
    DateTimeOffset? FinishedAt,
    FinishMethodContract? FinishMethodType,
    string? DeletionReason,
    [property: Required] UnsingedDeliveryDocumentContract[] DeliveryDocuments);