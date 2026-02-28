using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.SpecialInformations;

public record SpecialInformationResponse(
    [property: Required] Guid Id,
    [property: Required] string Title,
    [property: Required] string Description,
    [property: Required] bool HasFile
    );