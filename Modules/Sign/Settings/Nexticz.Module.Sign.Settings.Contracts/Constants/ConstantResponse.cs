using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Constants;

public record ConstantResponse(
    [property: Required] Guid Id, 
    [property: Required] string Key, 
    [property: Required] string Value, 
    [property: Required] ConstantTypeContract ConstantType, 
    string? Description);