using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Cuzk.Domain.AddressLocationAggregate.Events;


public record AddressLocationCreatedEvent(
    Guid Id,
    string AdmCode,
    string MunicipalityCode,
    string? MunicipalityName,
    string? MunicipalityDistrictCode,
    string? MunicipalityDistrictName,
    string? MomcCode,
    string? MomcName,
    string? PragueDistrictCode,
    string? PragueDistrictName,
    string? StreetCode,
    string? StreetName,
    string? DistrictCode,
    string? DistrictName,
    string? CountryCode,
    string? CountryName,
    string? SoType,
    string? NumberDescriptive,
    string? NumberReference,
    string? NumberReferenceChar,
    string? ZipCode,
    string? KrovakX,
    string? KrovakY,
    string? Latitude,
    string? Longitude,
    string? Altitude,
    string Slug,
    DateTimeOffset? ValidFrom,
    DateTimeOffset CreatedAt) : IMartenEvent;