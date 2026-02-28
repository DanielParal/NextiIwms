using System.ComponentModel.DataAnnotations;
using Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.UnsignedLoadingDocuments;

public record UnsingedDeliveryDocumentContract(
    [property: Required] string Code,
    [property: Required] string LoadingDocumentCode,
    [property: Required] string PartnerCode,
    [property: Required] string PartnerNameShort,
    [property: Required] string DeliveryMethodCode,
    string? DeliveryMethodName,
    [property: Required] string WarehouseCode,
    [property: Required] string PartnersOrderNumber,
    [property: Required] string OperationalUnitCode,
    [property: Required] string OperationalUnitName,
    [property: Required] int RequestedPrintCopiesCount,
    decimal? WeightCalculated,
    int? AdrPoints,
    string? RznoCode,
    string? CombinedRznoCode,
    string? SigningDeviceCode,
    string? SentToSigningDeviceByUserName,
    DateTimeOffset? SentToSigningDeviceAt,
    DateTimeOffset? FinishedAt,
    FinishMethodContract? FinishMethodType,
    string? DeletionReason);