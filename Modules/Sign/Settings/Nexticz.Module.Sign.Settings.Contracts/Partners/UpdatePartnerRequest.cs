using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Partners;

public record UpdatePartnerRequest([property: Required] string Name);