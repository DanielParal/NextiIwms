namespace Nexticz.Module.Sign.Settings.Application.Partners.ImportExport;

internal class PartnerItem(
    string code, 
    string name,
    int rowNumber)
{
    public string Code { get; } = code.ToUpperInvariant();
    public string Name { get; } = name;
    public int RowNumber { get; } = rowNumber;
}