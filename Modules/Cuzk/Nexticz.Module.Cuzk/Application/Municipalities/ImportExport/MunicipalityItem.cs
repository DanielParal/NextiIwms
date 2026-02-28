namespace Nexticz.Module.Cuzk.Application.Municipalities.ImportExport;


internal class MunicipalityItem(
    string code,
    string? name,
    string? status,
    string? pouCode,
    string? pouName,
    string? orpCode,
    string? orpName,
    string? districtCode,
    string? districtName,
    string? vuscCode,
    string? vuscName,
    int rowNumber)
{
    public string Code { get; } = code.ToUpperInvariant();
    public string? Name { get; } = name;
    public string? Status { get; } = status;
    public string? PouCode { get; } = pouCode;
    public string? PouName { get; } = pouName;
    public string? OrpCode { get; } = orpCode;
    public string? OrpName { get; } = orpName;
    public string? DistrictCode { get; } = districtCode;
    public string? DistrictName { get; } = districtName;
    public string? VuscCode { get; } = vuscCode;
    public string? VuscName { get; } = vuscName;
    public int RowNumber { get; } = rowNumber;
}