
namespace Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

public class PackagingCodeQuantity(string packagingCode, int quantity)
{
    public string PackagingCode { get; private set; } = packagingCode.ToUpperInvariant();
    public int Quantity { get; private set; } = quantity;
}