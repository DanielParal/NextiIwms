using DotSpatial.Projections;
using ErrorOr;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate.Events;

namespace Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

public class AddressLocation : AggregateRoot
{
    public string AdmCode { get; private set; }
    public string MunicipalityCode { get; private set; }
    public string? MunicipalityName { get; private set; }
    public string? MunicipalityDistrictCode { get; private set; }
    public string? MunicipalityDistrictName { get; private set; }
    public string? MomcCode { get; private set; }
    public string? MomcName { get; private set; }
    public string? PragueDistrictCode { get; private set; }
    public string? PragueDistrictName { get; private set; }
    public string? StreetCode { get; private set; }
    public string? StreetName { get; private set; }
    public string? DistrictCode { get; private set; }
    public string? DistrictName { get; private set; }
    public string? CountryCode { get; private set; }
    public string? CountryName { get; private set; }
    public string? SoType { get; private set; }
    public string? NumberDescriptive { get; private set; }
    public string? NumberReference { get; private set; }
    public string? NumberReferenceChar { get; private set; }
    public string? ZipCode { get; private set; }
    public string? KrovakX { get; private set; }
    public string? KrovakY { get; private set; }
    public string? Latitude { get; private set; }
    public string? Longitude { get; private set; }
    public string? Altitude { get; private set; }
    public string Slug { get; private set; }
    public DateTimeOffset? ValidFrom { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private AddressLocation() {}
    
    private AddressLocation(
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
        string? altitude,
        string slug,
        DateTimeOffset? validFrom,
        DateTimeOffset createdAt,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        AdmCode = admCode;
        MunicipalityCode = municipalityCode;
        MunicipalityName = municipalityName;
        MunicipalityDistrictCode = municipalityDistrictCode;
        MunicipalityDistrictName = municipalityDistrictName;
        MomcCode = momcCode;
        MomcName = momcName;
        PragueDistrictCode = pragueDistrictCode;
        PragueDistrictName = pragueDistrictName;
        StreetCode = streetCode;
        StreetName = streetName;
        DistrictCode = districtCode;
        DistrictName = districtName;
        CountryCode = countryCode;
        CountryName = countryName;
        SoType = soType;
        NumberDescriptive = numberDescriptive;
        NumberReference = numberReference;
        NumberReferenceChar = numberReferenceChar;
        ZipCode = zipCode;
        KrovakX = krovakX;
        KrovakY = krovakY;
        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
        Slug = slug;
        ValidFrom = validFrom;
        CreatedAt = createdAt;
    }

    public static ErrorOr<AddressLocation> CreateFrom(
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
        string? altitude,
        DateTimeOffset? validFrom,
        DateTimeOffset createdAt)
    {
        var validation = IsValid(admCode, municipalityCode);

        if (validation.IsError)
            return validation.Errors;
        
        var slug = CreateSlug(streetName ?? "", numberDescriptive ?? "", municipalityName ?? "", districtName ?? "");
        
        return new AddressLocation(admCode, municipalityCode, municipalityName, municipalityDistrictCode,
            municipalityDistrictName, momcCode, momcName, pragueDistrictCode, pragueDistrictName, streetCode,
            streetName, districtCode, districtName, countryCode, countryName, soType, numberDescriptive, 
            numberReference, numberReferenceChar, zipCode, krovakX, krovakY, latitude, longitude, 
            altitude, slug, validFrom, createdAt);
    }

    public bool HasSamePropertiesAs(
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
        string? altitude,
        DateTimeOffset? validFrom)
    {
        if (AdmCode != admCode) return false;
        if (MunicipalityCode != municipalityCode) return false;
        if (MunicipalityName != municipalityName) return false;
        if (MunicipalityDistrictCode != municipalityDistrictCode) return false;
        if (MunicipalityDistrictName != municipalityDistrictName) return false;
        if (MomcCode != momcCode) return false;
        if (MomcName != momcName) return false;
        if (PragueDistrictCode != pragueDistrictCode) return false;
        if (PragueDistrictName != pragueDistrictName) return false;
        if (StreetCode != streetCode) return false;
        if (StreetName != streetName) return false;
        if (DistrictCode != districtCode) return false;
        if (DistrictName != districtName) return false;
        if (CountryCode != countryCode) return false;
        if (CountryName != countryName) return false;
        if (SoType != soType) return false;
        if (NumberDescriptive != numberDescriptive) return false;
        if (NumberReference != numberReference) return false;
        if (NumberReferenceChar != numberReferenceChar) return false;
        if (ZipCode != zipCode) return false;
        if (KrovakX != krovakX) return false;
        if (KrovakY != krovakY) return false;
        if (Latitude != latitude) return false;
        if (Longitude != longitude) return false;
        if (Altitude != altitude) return false;
        if (ValidFrom != validFrom) return false;
        
        return true;
    }

    public ErrorOr<Success> Update(
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
        string? altitude,
        DateTimeOffset? validFrom,
        DateTimeOffset createdAt)
    {
        var validation = IsValid(admCode, municipalityCode);

        if (validation.IsError)
            return validation.Errors;
        
        var slug = CreateSlug(streetName ?? "", numberDescriptive ?? "", municipalityName ?? "", districtName ?? "");
        
        AdmCode = admCode;
        MunicipalityCode = municipalityCode;
        MunicipalityName = municipalityName;
        MunicipalityDistrictCode = municipalityDistrictCode;
        MunicipalityDistrictName = municipalityDistrictName;
        MomcCode = momcCode;
        MomcName = momcName;
        PragueDistrictCode = pragueDistrictCode;
        PragueDistrictName = pragueDistrictName;
        StreetCode = streetCode;
        StreetName = streetName;
        DistrictCode = districtCode;
        DistrictName = districtName;
        CountryCode = countryCode;
        CountryName = countryName;
        SoType = soType;
        NumberDescriptive = numberDescriptive;
        NumberReference = numberReference;
        NumberReferenceChar = numberReferenceChar;
        ZipCode = zipCode;
        KrovakX = krovakX;
        KrovakY = krovakY;
        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
        Slug = slug;
        ValidFrom = validFrom;
        CreatedAt = createdAt;
        
        return Result.Success;
    }

    public static (double Latitude, double Longitude) ConvertKrovakToLatLon(double krovakX, double krovakY)
    {
        var krovak = KnownCoordinateSystems.Projected.NationalGrids.SJTSKKrovakEastNorth;
        var wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;

        double[] krovakCoords = [-Math.Abs(krovakY), -Math.Abs(krovakX)];
        double[] z = [0];

        Reproject.ReprojectPoints(krovakCoords, z, krovak, wgs84, 0, 1);
        
        return (krovakCoords[1], krovakCoords[0]);
    }
    
    private static string CreateSlug(string streetName, string numberDescriptive, string municipalityName, string districtName)
    {
        return streetName + " " + numberDescriptive + ", " + municipalityName + " " + districtName;
    }
    
    private static ErrorOr<Success> IsValid(string admCode, string municipalityCode)
    {
        if (string.IsNullOrWhiteSpace(admCode))
            return AddressLocationDomainErrors.ValidationAdmCodeIsRequired;
        
        if (string.IsNullOrWhiteSpace(municipalityCode))
            return AddressLocationDomainErrors.ValidationMunicipalityCodeIsRequired;
        
        return Result.Success;
    }

    public void Apply(AddressLocationCreatedEvent @event)
    {
        AdmCode = @event.AdmCode;
        MunicipalityCode = @event.MunicipalityCode;
        MunicipalityName = @event.MunicipalityName;
        MunicipalityDistrictCode = @event.MunicipalityDistrictCode;
        MunicipalityDistrictName = @event.MunicipalityDistrictName;
        MomcCode = @event.MomcCode;
        MomcName = @event.MomcName;
        PragueDistrictCode = @event.PragueDistrictCode;
        PragueDistrictName = @event.PragueDistrictName;
        StreetCode = @event.StreetCode;
        StreetName = @event.StreetName;
        DistrictCode = @event.DistrictCode;
        DistrictName = @event.DistrictName;
        CountryCode = @event.CountryCode;
        CountryName = @event.CountryName;
        SoType = @event.SoType;
        NumberDescriptive = @event.NumberDescriptive;
        NumberReference = @event.NumberReference;
        NumberReferenceChar = @event.NumberReferenceChar;
        ZipCode = @event.ZipCode;
        KrovakX = @event.KrovakX;
        KrovakY = @event.KrovakY;
        Latitude = @event.Latitude;
        Longitude = @event.Longitude;
        Altitude = @event.Altitude;
        Slug = @event.Slug;
        ValidFrom = @event.ValidFrom;
        CreatedAt = @event.CreatedAt;
    }
    
    public void Apply(AddressLocationUpdatedEvent @event)
    {
        AdmCode = @event.AdmCode;
        MunicipalityCode = @event.MunicipalityCode;
        MunicipalityName = @event.MunicipalityName;
        MunicipalityDistrictCode = @event.MunicipalityDistrictCode;
        MunicipalityDistrictName = @event.MunicipalityDistrictName;
        MomcCode = @event.MomcCode;
        MomcName = @event.MomcName;
        PragueDistrictCode = @event.PragueDistrictCode;
        PragueDistrictName = @event.PragueDistrictName;
        StreetCode = @event.StreetCode;
        StreetName = @event.StreetName;
        DistrictCode = @event.DistrictCode;
        DistrictName = @event.DistrictName;
        CountryCode = @event.CountryCode;
        CountryName = @event.CountryName;
        SoType = @event.SoType;
        NumberDescriptive = @event.NumberDescriptive;
        NumberReference = @event.NumberReference;
        NumberReferenceChar = @event.NumberReferenceChar;
        ZipCode = @event.ZipCode;
        KrovakX = @event.KrovakX;
        KrovakY = @event.KrovakY;
        Latitude = @event.Latitude;
        Longitude = @event.Longitude;
        Altitude = @event.Altitude;
        Slug = @event.Slug;
        ValidFrom = @event.ValidFrom;
    }
}