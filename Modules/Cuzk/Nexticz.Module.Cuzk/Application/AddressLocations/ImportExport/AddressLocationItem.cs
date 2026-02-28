namespace Nexticz.Module.Cuzk.Application.AddressLocations.ImportExport;

internal class AddressLocationItem(
    string admCode,
    string municipalityCode,
    string? municipalityName,
    string? municipalityDistrictCode,
    string? municipalityDistrictName,
    string? momcCode,
    string? momcName,
    string? pragueDistrictCode,
    string? pragueDistrictName,
    string? streetCode,
    string? streetName,
    string? districtCode,
    string? districtName,
    string? countryCode,
    string? countryName,
    string? soType,
    string? numberDescriptive,
    string? numberReference,
    string? numberReferenceChar,
    string? zipCode,
    string? krovakX,
    string? krovakY,
    string? latitude,
    string? longitude,
    DateTime? validFrom,
    int rowNumber)
{
    public string AdmCode { get; } = admCode.ToUpperInvariant();
    public string MunicipalityCode { get; } = municipalityCode.ToUpperInvariant();
    public string? MunicipalityName { get; } = municipalityName;
    public string? MunicipalityDistrictCode { get; } = municipalityDistrictCode;
    public string? MunicipalityDistrictName { get; } = municipalityDistrictName;
    public string? MomcCode { get; } = momcCode;
    public string? MomcName { get; } = momcName;
    public string? PragueDistrictCode { get; } = pragueDistrictCode;
    public string? PragueDistrictName { get; } = pragueDistrictName;
    public string? StreetCode { get; } = streetCode;
    public string? StreetName { get; } = streetName;
    public string? DistrictCode { get; } = districtCode;
    public string? DistrictName { get; } = districtName;
    public string? CountryCode { get; } = countryCode;
    public string? CountryName { get; } = countryName;
    public string? SoType { get; } = soType;
    public string? NumberDescriptive { get; } = numberDescriptive;
    public string? NumberReference { get; } = numberReference;
    public string? NumberReferenceChar { get; } = numberReferenceChar;
    public string? ZipCode { get; } = zipCode;
    public string? KrovakX { get; } = krovakX;
    public string? KrovakY { get; } = krovakY;
    public string? Latitude { get; } = latitude;
    public string? Longitude { get; } = longitude;
    public DateTime? ValidFrom { get; } = validFrom;
    public int RowNumber { get; } = rowNumber;
}