using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.ImportExport;


internal class PackagingItem(
    string code,
    string packagingTypeCode,
    string depositorCode,
    string packagingCirculationCode,
    string customerNumber,
    string name,
    bool mustBeWashed,
    decimal depth,
    decimal width,
    decimal height,
    decimal weight,
    List<WashingMachineSpeed> washingMachineSpeeds,
    int rowNumber)
{
    public string Code { get; private set; } = code.ToUpperInvariant();
    public string PackagingTypeCode { get; private set; } = packagingTypeCode.ToUpperInvariant();
    public string DepositorCode { get; private set; } = depositorCode.ToUpperInvariant();
    public string PackagingCirculationCode { get; private set; } = packagingCirculationCode.ToUpperInvariant();
    public string CustomerNumber { get; private set; } = customerNumber.ToUpperInvariant();
    public string Name { get; private set; } = name;
    public bool MustBeWashed { get; private set; } = mustBeWashed;
    public decimal Depth { get; private set; } = depth;
    public decimal Width { get; private set; } = width;
    public decimal Height { get; private set; } = height;
    public decimal Weight { get; private set; } = weight;
    public int RowNumber { get; private set; } = rowNumber;
    public List<WashingMachineSpeed> WashingMachineSpeeds { get; private set; } = washingMachineSpeeds;
}