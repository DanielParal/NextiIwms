namespace Nexticz.Module.Sign.Settings.Contracts.EmailTemplates;

public record EmailTemplateContract(
    Guid Id, string Code, string Subject, string HtmlBody, string TextBody);