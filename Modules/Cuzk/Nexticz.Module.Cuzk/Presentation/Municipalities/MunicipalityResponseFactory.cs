using Nexticz.Module.Cuzk.Contracts.Municipalities;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

namespace Nexticz.Module.Cuzk.Presentation.Municipalities;

internal static class MunicipalityResponseFactory
{
    public static MunicipalityResponse Create(Municipality municipality)
    {
        return new MunicipalityResponse()
        {
            Id = municipality.Id,
            MunicipalityCode = municipality.Code,
            MunicipalityName = municipality.Name,
            MunicipalityStatus = municipality.Status,
            PouCode = municipality.PouCode,
            PouName = municipality.PouName,
            OrpCode = municipality.OrpCode, 
            OrpName = municipality.OrpName,
            DistrictCode = municipality.DistrictCode,
            DistrictName = municipality.DistrictName,
            VuscCode = municipality.VuscCode,
            VuscName = municipality.VuscName,
            Created = municipality.CreatedAt.DateTime,
            ShouldImportAddressLocation = municipality.ShouldImportAddressLocation,
        };
    }
}