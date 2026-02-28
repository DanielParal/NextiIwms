using System.ComponentModel.DataAnnotations;
using Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.SignedLoadingDocuments;

public record SignedLoadingDocumentResponse(
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
    DateTimeOffset? FinishedAt,
    string? FinishedByUserName,
    string? FinishedByUserFullName,
    FinishMethodContract? FinishMethodType,
    [property: Required] bool IsDocumentFullyFinished,
    string? DeletionReason,
    [property: Required] SingedDeliveryDocumentContract[] DeliveryDocuments);