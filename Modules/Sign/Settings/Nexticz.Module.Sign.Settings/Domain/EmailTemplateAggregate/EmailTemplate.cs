using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate.Events;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate;

public class EmailTemplate : AggregateRoot
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Subject { get; private set; }
    public string HtmlBody { get; private set; }
    public string TextBody { get; private set; }

    // We need private constructor due to Marten deserialization
    private EmailTemplate() {}

    private EmailTemplate(
        string code,
        string name,
        string subject,
        string htmlBody,
        string textBody,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Name = name;
        Subject = subject;
        HtmlBody = htmlBody;
        TextBody = textBody;
    }

    public static ErrorOr<EmailTemplate> CreateNew(
        string code,
        string name,
        string subject,
        string htmlBody,
        string textBody)
    {
        var parseResult = Enum.TryParse(code, true, out EmailTemplateType _);
        if (!parseResult)
            return EmailTemplateDomainErrors.ValidationCodeIsNotRecognizedInListOfTemplates;
        
        var validationResult = IsValid(name, subject, htmlBody, textBody);
        if (validationResult.IsError)
            return validationResult.Errors;
        
        return new EmailTemplate(code, name, subject, htmlBody, textBody);
    }

    public ErrorOr<Success> Update(
        string name,
        string subject,
        string htmlBody,
        string textBody)
    {
        var isValidRequest = IsValid(name, subject, htmlBody, textBody);

        if (isValidRequest.IsError)
            return isValidRequest.Errors;
        
        Subject = subject;
        HtmlBody = htmlBody;
        TextBody = textBody;
        
        return Result.Success;
    }
    
    private static ErrorOr<Success> IsValid(
        string name,
        string subject,
        string htmlBody,
        string textBody)
    {
        
        if (string.IsNullOrWhiteSpace(name))
            return EmailTemplateDomainErrors.ValidationNameIsRequired;
        
        if (string.IsNullOrWhiteSpace(subject))
            return EmailTemplateDomainErrors.ValidationSubjectIsRequired;
        
        if (string.IsNullOrWhiteSpace(htmlBody))
            return EmailTemplateDomainErrors.ValidationHtmlBodyIsRequired;
        
        if (string.IsNullOrWhiteSpace(textBody))
            return EmailTemplateDomainErrors.ValidationTextBodyIsRequired;
        
        return Result.Success;
    }

    public void Apply(EmailTemplateCreatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
        Subject = @event.Subject;
        HtmlBody = @event.HtmlBody;
        TextBody = @event.TextBody;
    }
    
    public void Apply(EmailTemplateUpdatedEvent @event)
    {
        Name = @event.Name;
        Subject = @event.Subject;
        HtmlBody = @event.HtmlBody;
        TextBody = @event.TextBody;
    }
}