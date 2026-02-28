namespace Nexticz.Module.Mmo.Settings.Application.Kits.ImportExport;

internal class KitItem(
    string kitNumber,
    string depositorCode,
    string kitTypeCode,
    string kitSapDefinitionCode,
    string manufactureCode,
    string definingPackagingCode,
    string note,
    int dryingTime,
    bool hasKitInstructionFile,
    int rowNumber)
{
    public string KitCode { get; } = $"{depositorCode.ToUpperInvariant()}{kitTypeCode.ToUpperInvariant()}{kitNumber}"; 
    public string KitNumber { get; private set; } = kitNumber.ToUpperInvariant();
    public string DepositorCode { get; private set; } = depositorCode.ToUpperInvariant();
    public string KitTypeCode { get; private set; } = kitTypeCode.ToUpperInvariant();
    public string KitSapDefinitionCode { get; private set; } = kitSapDefinitionCode.ToUpperInvariant();
    public string ManufactureCode { get; private set; } = manufactureCode.ToUpperInvariant();
    public string DefiningPackagingCode { get; private set; } = definingPackagingCode.ToUpperInvariant();
    public string Note { get; private set; } = note;
    public int DryingTime { get; private set; } = dryingTime;
    public bool HasKitInstructionFile { get; private set; } = hasKitInstructionFile;
    public int RowNumber { get; private set; } = rowNumber;
    public List<(string Code, int Quantity)> PackagingCodeQuantities { get; private set; } = [];
    
    public void AddPackagingCodeQuantity(string code, int quantity)
    {
        PackagingCodeQuantities.Add((code.ToUpperInvariant(), quantity));
    }
}