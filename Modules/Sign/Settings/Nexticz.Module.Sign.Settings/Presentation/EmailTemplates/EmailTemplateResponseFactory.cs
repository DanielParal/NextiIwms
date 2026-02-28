using Nexticz.Module.Sign.Settings.Contracts.EmailTemplates;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.EmailTemplates;

internal static class EmailTemplateResponseFactory
{
    public static EmailTemplateResponse Create(EmailTemplate emailTemplate)
    {
        return new EmailTemplateResponse(
            emailTemplate.Id,
            emailTemplate.Code,
            emailTemplate.Name,
            emailTemplate.Subject,
            emailTemplate.HtmlBody,
            emailTemplate.TextBody);
    }
}