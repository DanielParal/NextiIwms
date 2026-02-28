namespace Nexticz.Module.Cuzk.Application.Municipalities.Commands.CreateUpdateBulkMunicipalities;

internal record CreateUpdateBulkMunicipalitiesRequest(
    string MunicipalityCode,
    string? MunicipalityName,
    string? MunicipalityStatus,
    string? PouCode,
    string? PouName,
    string? OrpCode,
    string? OrpName,
    string? DistrictCode,
    string? DistrictName,
    string? VuscCode,
    string? VuscName,
    bool ShouldImportAddressLocation
    );