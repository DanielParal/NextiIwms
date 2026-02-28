using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Domain.Views;

public class UnsignedDeliveryDocument
{
    public string Code { get; set; }
    public string LoadingDocumentCode { get; set; }
    public string PartnerCode { get; set; }
    public string PartnerNameShort { get; set; }
    public string DeliveryMethodCode { get; set; }
    public string? DeliveryMethodName { get; set; }
    public string WarehouseCode { get; set; }
    public string PartnersOrderNumber { get; set; }
    public string OperationalUnitCode { get; set; }
    public string OperationalUnitName { get; set; }
    public decimal? WeightCalculated { get; set; }
    public int? AdrPoints { get; set; }
    public string? RznoCode { get; set; }
    public string? CombinedRznoCode { get; set; } 
    public int RequestedPrintCopiesCount { get; set; }
    public string? SigningDeviceCode { get; set; }
    public DateTimeOffset? SentToSigningDeviceAt { get; set; }
    public string? SentToSigningDeviceByUserName { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public FinishMethod? FinishMethodType { get; set; }
    public string? DeletionReason { get; set; }

    public UnsignedDeliveryDocument(
        string code,
        string loadingDocumentCode,
        string partnerCode,
        string partnerNameShort,
        string deliveryMethodCode,
        string? deliveryMethodName,
        string warehouseCode,
        string partnersOrderNumber,
        string operationalUnitCode,
        string operationalUnitName,
        decimal? weightCalculated,
        int? adrPoints,
        string? rznoCode,
        string? combinedRznoCode,
        int requestedPrintCopiesCount,       
        string? signingDeviceCode,
        DateTimeOffset? sentToSigningDeviceAt,
        string? sentToSigningDeviceByUserName,
        DateTimeOffset? finishedAt,
        FinishMethod? finishMethodType,
        string? deletionReason)
    {
        Code = code;
        LoadingDocumentCode = loadingDocumentCode;
        PartnerCode = partnerCode;
        PartnerNameShort = partnerNameShort;
        DeliveryMethodCode = deliveryMethodCode;
        DeliveryMethodName = deliveryMethodName;
        WarehouseCode = warehouseCode;
        PartnersOrderNumber = partnersOrderNumber;
        OperationalUnitCode = operationalUnitCode;
        OperationalUnitName = operationalUnitName;
        WeightCalculated = weightCalculated;
        AdrPoints = adrPoints;
        RznoCode = rznoCode;
        CombinedRznoCode = combinedRznoCode;
        RequestedPrintCopiesCount = requestedPrintCopiesCount;       
        SigningDeviceCode = signingDeviceCode;
        SentToSigningDeviceAt = sentToSigningDeviceAt;
        SentToSigningDeviceByUserName = sentToSigningDeviceByUserName;
        FinishedAt = finishedAt;
        FinishMethodType = finishMethodType;       
        DeletionReason = deletionReason;
    }
}