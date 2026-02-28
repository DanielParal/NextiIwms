

using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;

public record CreateDocumentTemplateRequest(
    [property: Required] string Code, 
    [property: Required] TextOffsetContract[] TextOffsets, 
    [property: Required] TextBackgroundContract[] TextBackgrounds);