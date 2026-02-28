namespace Nexticz.Module.Vh.Contracts.Assortments;

public class CreateAssortmentRequest
{
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required bool Receipt { get; set; }
    public decimal ReceiptCoefficient { get; set; }
    public required bool Dispatch { get; set; }
    public decimal DispatchCoefficient { get; set; }
    public required bool Packaging { get; set; }
    public decimal PackagingCoefficient { get; set; }
}