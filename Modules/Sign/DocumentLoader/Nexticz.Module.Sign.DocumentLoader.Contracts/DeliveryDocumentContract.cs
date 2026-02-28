namespace Nexticz.Module.Sign.DocumentLoader.Contracts;

public record DeliveryDocumentContract(
    string Code,
    string LoadingDocumentCode,
    string DepositorCode,
    string PartnerCode,
    string PartnerNameShort,
    string DeliveryMethodCode,
    string WarehouseCode,
    string PartnersOrderNumber,
    string OperationalUnitCode,
    string OperationalUnitName,
    DateTime IssueDate,
    string? RznoCode,
    string? CombinedRznoCode,
    decimal? WeightCalculated,
    int? AdrPoints);