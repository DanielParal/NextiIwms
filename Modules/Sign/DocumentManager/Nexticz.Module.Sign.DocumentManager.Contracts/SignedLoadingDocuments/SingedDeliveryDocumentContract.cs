using System.ComponentModel.DataAnnotations;
using Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.SignedLoadingDocuments;

public record SingedDeliveryDocumentContract(
    [property: Required] string Code,
    [property: Required] string LoadingDocumentCode,
    [property: Required] string PartnerCode,
    [property: Required] string PartnerNameShort,
    [property: Required] string DeliveryMethodCode,
    string? DeliveryMethodName,
    string? LicensePlate,
    string? DriverName,
    [property: Required] string WarehouseCode,
    [property: Required] string PartnersOrderNumber,
    [property: Required] string OperationalUnitCode,
    [property: Required] string OperationalUnitName,
    decimal? WeightCalculated,
    int? AdrPoints,
    string? RznoCode,
    string? CombinedRznoCode,
    [property: Required] DateTimeOffset FinishedAt,
    [property: Required] string FinishedByUserName,
    string? FinishedByUserFullName,
    [property: Required] FinishMethodContract FinishMethodType,
    string? DeletionReason);