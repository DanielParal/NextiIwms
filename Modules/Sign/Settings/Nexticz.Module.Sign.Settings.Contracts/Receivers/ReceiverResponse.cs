using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Receivers;

public record ReceiverResponse(
    [property: Required] Guid Id, 
    [property: Required] string Code, 
    [property: Required] string Name,
    [property: Required] string PartnerCode);