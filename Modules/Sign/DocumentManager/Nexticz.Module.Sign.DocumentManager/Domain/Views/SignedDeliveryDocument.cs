using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Domain.Views;

public class SignedDeliveryDocument
{
    public string Code { get; set; }
    public string LoadingDocumentCode { get; set; }
    public string PartnerCode { get; set; }
    public string PartnerNameShort { get; set; }
    public string DeliveryMethodCode { get; set; }
    public string? DeliveryMethodName { get; set; }
    public string? LicensePlate { get; set; }
    public string? DriverName { get; set; }
    public string WarehouseCode { get; set; }
    public string PartnersOrderNumber { get; set; }
    public string OperationalUnitCode { get; set; }
    public string OperationalUnitName { get; set; }
    public decimal? WeightCalculated { get; set; }
    public int? AdrPoints { get; set; }
    public string? RznoCode { get; set; }
    public string? CombinedRznoCode { get; set; } 
    public string? SigningDeviceCode { get; set; }
    public int PrintedCopiesCount { get; set; }
    public DateTimeOffset FinishedAt { get; set; }
    public string FinishedByUserName { get; set; }
    public string? FinishedByUserFullName { get; set; }
    public FinishMethod FinishMethodType { get; set; }
    public string? DeletionReason { get; set; }

    public SignedDeliveryDocument(
        string code,
        string loadingDocumentCode,
        string partnerCode,
        string partnerNameShort,
        string deliveryMethodCode,
        string? deliveryMethodName,
        string? licensePlate,
        string? driverName,
        string warehouseCode,
        string partnersOrderNumber,
        string operationalUnitCode,
        string operationalUnitName,
        decimal? weightCalculated,
        int? adrPoints,
        string? rznoCode,
        string? combinedRznoCode,
        string? signingDeviceCode,
        int printedCopiesCount,
        DateTimeOffset finishedAt,
        string finishedByUserName,
        string? finishedByUserFullName,
        FinishMethod finishMethodType,
        string? deletionReason)
    {
        Code = code;
        LoadingDocumentCode = loadingDocumentCode;
        PartnerCode = partnerCode;
        PartnerNameShort = partnerNameShort;
        DeliveryMethodCode = deliveryMethodCode;
        DeliveryMethodName = deliveryMethodName; 
        LicensePlate = licensePlate;
        DriverName = driverName;       
        WarehouseCode = warehouseCode;
        PartnersOrderNumber = partnersOrderNumber;
        OperationalUnitCode = operationalUnitCode;
        OperationalUnitName = operationalUnitName;
        WeightCalculated = weightCalculated;
        AdrPoints = adrPoints;
        RznoCode = rznoCode;
        CombinedRznoCode = combinedRznoCode;       
        SigningDeviceCode = signingDeviceCode;
        PrintedCopiesCount = printedCopiesCount;
        FinishedAt = finishedAt;
        FinishedByUserName = finishedByUserName;
        FinishedByUserFullName = finishedByUserFullName;       
        FinishMethodType = finishMethodType;  
        DeletionReason = deletionReason;
    }
}