using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;

public record DocumentTemplateResponse(
    [property: Required] Guid Id, 
    [property: Required] string Code, 
    [property: Required] TextOffsetContract[] TextOffsets, 
    [property: Required] TextBackgroundContract[] TextBackgrounds);