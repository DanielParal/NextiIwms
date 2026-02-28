using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.EmailTemplates;

public record UpdateEmailTemplateRequest(
    [property: Required] string Name,
    [property: Required] string Subject,
    [property: Required] string HtmlBody,
    [property: Required] string TextBody);