using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Domain.Views;

public class UnsignedLoadingDocumentView
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public int? GateNumber { get; set; }
    public string DepositorCode { get; set; }
    public string DeliveryMethodCode { get; set; }
    public string? DeliveryMethodName { get; set; }
    public string? LicensePlate { get; set; }
    public string? DriverName { get; set; }
    public decimal? Weight { get; set; }
    public int? AdrPoints { get; set; }
    public string? LoadingLocation { get; set; }
    public string LoadingInWmsFinishedBy { get; set; }
    public DateTimeOffset LoadingInWmsFinishedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int RequestedPrintCopiesCount { get; set; }
    public string? SigningDeviceCode { get; set; }
    public DateTimeOffset? SentToSigningDeviceAt { get; set; }
    public string? SentToSigningDeviceByUserName { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public FinishMethod? FinishMethodType { get; set; }
    public string? DeletionReason { get; set; }
    public UnsignedDeliveryDocument[] DeliveryDocuments { get; set; } = [];
}