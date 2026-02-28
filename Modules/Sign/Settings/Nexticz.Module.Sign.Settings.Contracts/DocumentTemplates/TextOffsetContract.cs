using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;

public record TextOffsetContract(
    [property: Required] string Name,
    [property: Required] double Left,
    [property: Required] double Bottom,
    [property: Required] double Width,
    [property: Required] double Height);