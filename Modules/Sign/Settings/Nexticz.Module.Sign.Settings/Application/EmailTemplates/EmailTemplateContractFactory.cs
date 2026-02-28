using Nexticz.Module.Sign.Settings.Contracts.EmailTemplates;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailTemplates;

internal static class EmailTemplateContractFactory
{
    public static EmailTemplateContract Create(EmailTemplate emailTemplate)
    {
        return new EmailTemplateContract(emailTemplate.Id, emailTemplate.Code, emailTemplate.Subject, emailTemplate.HtmlBody, emailTemplate.TextBody);
    }
}