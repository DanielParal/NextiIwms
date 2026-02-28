using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Domain.Views;

public class SignedLoadingDocumentView
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
    public string? SigningDeviceCode { get; set; }
    public bool IsDocumentFullyFinished { get; set; }
    public int PrintedCopiesCount { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public string? FinishedByUserName { get; set; }
    public string? FinishedByUserFullName { get; set; }
    public FinishMethod? FinishMethodType { get; set; }
    public string? DeletionReason { get; set; }
    public List<SignedDeliveryDocument> DeliveryDocuments { get; set; } = [];
}