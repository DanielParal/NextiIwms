using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Cuzk.Contracts.AddressLocationsSlugs;

public record AddressLocationSlugResponse(
    [property: Required] Guid Id,
    [property: Required] string AdmCode,
    [property: Required] string Slug);