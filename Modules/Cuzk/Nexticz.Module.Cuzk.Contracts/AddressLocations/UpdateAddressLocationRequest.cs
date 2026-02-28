namespace Nexticz.Module.Cuzk.Contracts.AddressLocations;

public class UpdateAddressLocationRequest
{
    public required string MunicipalityCode { get; set; }
    public string? MunicipalityName { get; set; }
    public string? MunicipalityDistrictCode { get; set; }
    public string? MunicipalityDistrictName { get; set; }
    public string? MomcCode { get; set; }
    public string? MomcName { get; set; }
    public string? PragueDistrictCode { get; set; }
    public string? PragueDistrictName { get; set; }
    public string? StreetCode { get; set; }
    public string? StreetName { get; set; }
    public string? DistrictCode { get; set; }
    public string? DistrictName { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryName { get; set; }
    public string? SoType { get; set; }
    public string? NumberDescriptive { get; set; }
    public string? NumberReference { get; set; }
    public string? NumberReferenceChar { get; set; }
    public string? ZipCode { get; set; }
    public string? KrovakX { get; set; }
    public string? KrovakY { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public string? Altitude { get; set; }
    public DateTime? ValidFrom { get; set; }
}