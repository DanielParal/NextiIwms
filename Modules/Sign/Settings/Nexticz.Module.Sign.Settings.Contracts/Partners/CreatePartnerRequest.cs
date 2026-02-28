using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Partners;

public record CreatePartnerRequest([property: Required] string Code, [property: Required] string Name);