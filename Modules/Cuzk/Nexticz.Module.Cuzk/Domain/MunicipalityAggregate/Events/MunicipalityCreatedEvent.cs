using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Cuzk.Domain.MunicipalityAggregate.Events;

public record MunicipalityCreatedEvent(
    Guid Id,
    string Code,
    string? Name,
    string? Status,
    string? PouCode,
    string? PouName,
    string? OrpCode,
    string? OrpName,
    string? DistrictCode,
    string? DistrictName,
    string? VuscCode,
    string? VuscName,
    bool ShouldImportAddressLocation,
    DateTimeOffset CreatedAt) : IMartenEvent;