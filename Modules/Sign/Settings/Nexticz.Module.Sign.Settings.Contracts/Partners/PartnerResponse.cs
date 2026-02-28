using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Partners;

public record PartnerResponse(
    [property: Required] Guid Id, 
    [property: Required] string Code, 
    [property: Required] string Name);