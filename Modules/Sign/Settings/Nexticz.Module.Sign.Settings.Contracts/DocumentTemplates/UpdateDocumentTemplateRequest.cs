using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;

public record UpdateDocumentTemplateRequest(
    [property: Required] TextOffsetContract[] TextOffsets, 
    [property: Required] TextBackgroundContract[] TextBackgrounds);