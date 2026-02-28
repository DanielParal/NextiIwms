using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;

public record TextBackgroundContract(
    [property: Required] string Name,
    [property: Required] double XPositionOffset,
    [property: Required] double Width);