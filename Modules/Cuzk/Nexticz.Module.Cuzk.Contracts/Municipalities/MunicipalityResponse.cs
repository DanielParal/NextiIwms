namespace Nexticz.Module.Cuzk.Contracts.Municipalities;

public class MunicipalityResponse
{
    public Guid Id { get; set; }
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
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }
    public bool ShouldImportAddressLocation { get; set; }
}