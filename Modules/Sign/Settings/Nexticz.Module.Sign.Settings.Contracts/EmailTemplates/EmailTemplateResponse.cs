using System.ComponentModel.DataAnnotations;
using Marten.Schema.Identity;

namespace Nexticz.Module.Sign.Settings.Contracts.EmailTemplates;

public record EmailTemplateResponse(
    [property: Required] Guid Id,
    [property: Required] string Code,
    [property: Required] string Name,
    [property: Required] string Subject,
    [property: Required] string HtmlBody,
    [property: Required] string TextBody);