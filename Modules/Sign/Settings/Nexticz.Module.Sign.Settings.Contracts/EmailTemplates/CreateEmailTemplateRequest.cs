using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.EmailTemplates;

public record CreateEmailTemplateRequest(
    [property: Required] string Code,
    [property: Required] string Name,
    [property: Required] string Subject,
    [property: Required] string HtmlBody,
    [property: Required] string TextBody);