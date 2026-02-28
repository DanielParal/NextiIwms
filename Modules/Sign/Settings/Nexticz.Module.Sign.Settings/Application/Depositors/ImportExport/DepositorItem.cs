namespace Nexticz.Module.Sign.Settings.Application.Depositors.ImportExport;

internal class DepositorItem(
    string code, 
    string name,
    string depositorGroupCode,
    string deliveryTemplateCode,
    string loadingTemplateCode,
    int rowNumber)
{
    public string Code { get; } = code.ToUpperInvariant();
    public string Name { get; } = name;
    public string DepositorGroupCode { get; } = depositorGroupCode.ToUpperInvariant();
    public string DeliveryTemplateCode { get; } = deliveryTemplateCode.ToUpperInvariant();
    public string LoadingTemplateCode { get; } = loadingTemplateCode.ToUpperInvariant();   
    public int RowNumber { get; } = rowNumber;
}