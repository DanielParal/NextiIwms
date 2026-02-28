namespace Nexticz.Module.Cuzk.Contracts.Municipalities;

public class UpdateMunicipalityRequest
{
    public string? MunicipalityCode { get; set; }
    public string? MunicipalityName { get; set; }
    public string? MunicipalityStatus { get; set; }
    public string? PouCode { get; set; }
    public string? PouName { get; set; }
    public string? OrpCode { get; set; }
    public string? OrpName { get; set; }
    public string? DistrictCode { get; set; }
    public string? DistrictName { get; set; }
    public string? VuscCode { get; set; }
    public string? VuscName { get; set; }
    public bool ShouldImportAddressLocation { get; set; }
}