using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Constants;

public record UpdateConstantRequest(
    [property: Required] string Value,
    string? Description
    );