using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DocumentTemplates;

internal static class DocumentTemplateResponseFactory
{
    public static DocumentTemplateResponse Create(DocumentTemplate documentTemplate)
    {
        return new DocumentTemplateResponse(
            documentTemplate.Id,
            documentTemplate.Code,
            documentTemplate.TextOffsets.Select(x => new TextOffsetContract(x.Name, x.Left, x.Bottom, x.Width, x.Height)).ToArray(),
            documentTemplate.TextBackgrounds.Select(x => new TextBackgroundContract(x.Name, x.XPositionOffset, x.Width)).ToArray());
    }
}