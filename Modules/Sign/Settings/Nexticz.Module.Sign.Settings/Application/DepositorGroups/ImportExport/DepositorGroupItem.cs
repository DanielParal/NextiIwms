namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.ImportExport;

internal class DepositorGroupItem(
    string code, 
    string name,
    int rowNumber)
{
    public string Code { get; } = code.ToUpperInvariant();
    public string Name { get; } = name;
    public int RowNumber { get; } = rowNumber;
}