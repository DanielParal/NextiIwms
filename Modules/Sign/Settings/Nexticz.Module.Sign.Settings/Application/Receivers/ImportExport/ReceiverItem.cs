namespace Nexticz.Module.Sign.Settings.Application.Receivers.ImportExport;

internal class ReceiverItem(
    string code,
    string name,
    string partnerCode,
    int rowNumber)
{
    public string Code { get; } = code.ToUpperInvariant();
    public string Name { get; } = name;
    public string PartnerCode { get; } = partnerCode.ToUpperInvariant();
    public int RowNumber { get; } = rowNumber;
}