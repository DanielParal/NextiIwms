using Nexticz.Module.Cuzk.Contracts.AddressLocations;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Presentation.AddressLocations;

internal static class AddressLocationResponseFactory
{
    public static AddressLocationResponse Create(AddressLocation addressLocation)
    {
        return new AddressLocationResponse
        {
            Id = addressLocation.Id,
            AdmCode = addressLocation.AdmCode,
            MunicipalityCode = addressLocation.MunicipalityCode,
            MunicipalityName = addressLocation.MunicipalityName,
            MunicipalityDistrictCode = addressLocation.MunicipalityDistrictCode,
            MunicipalityDistrictName = addressLocation.MunicipalityDistrictName,
            MomcCode = addressLocation.MomcCode,
            MomcName = addressLocation.MomcName,
            PragueDistrictCode = addressLocation.PragueDistrictCode,
            PragueDistrictName = addressLocation.PragueDistrictName,
            StreetCode = addressLocation.StreetCode,
            StreetName = addressLocation.StreetName,
            DistrictCode = addressLocation.DistrictCode,
            DistrictName = addressLocation.DistrictName,
            CountryCode = addressLocation.CountryCode,
            CountryName = addressLocation.CountryName,
            SoType = addressLocation.SoType,
            NumberDescriptive = addressLocation.NumberDescriptive,
            NumberReference = addressLocation.NumberReference,
            NumberReferenceChar = addressLocation.NumberReferenceChar,
            ZipCode = addressLocation.ZipCode,
            KrovakX = addressLocation.KrovakX,
            KrovakY = addressLocation.KrovakY,
            Latitude = addressLocation.Latitude,
            Longitude = addressLocation.Longitude,
            Altitude = addressLocation.Altitude,
            Slug = addressLocation.Slug,
            ValidFrom = addressLocation.ValidFrom?.DateTime,
            Created = addressLocation.CreatedAt.DateTime
        };
    }
}