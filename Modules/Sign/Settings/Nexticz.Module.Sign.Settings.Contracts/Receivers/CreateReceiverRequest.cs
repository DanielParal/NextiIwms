using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Receivers;

public record CreateReceiverRequest(
    [property: Required] string Code, 
    [property: Required] string PartnerCode, 
    [property: Required] string Name);