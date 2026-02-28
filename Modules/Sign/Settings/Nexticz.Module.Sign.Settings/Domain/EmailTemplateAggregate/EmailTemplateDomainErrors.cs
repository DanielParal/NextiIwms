using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate;

internal abstract class EmailTemplateDomainErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-domain-emailTemplate-";
    
    public static Error ValidationSubjectIsRequired = Error.Validation(
        ComponentSlug + "ValidationSubjectIsRequired",
        "Předmět emailu je povinné pole."
    );
    
    public static Error ValidationNameIsRequired = Error.Validation(
        ComponentSlug + "ValidationNameIsRequired",
        "Jméno emailu je povinné pole."
    );
    
    public static Error ValidationHtmlBodyIsRequired = Error.Validation(
        ComponentSlug + "ValidationHtmlBodyIsRequired",
        "Html tělo emailu je povinné pole."
    );
    
    public static Error ValidationTextBodyIsRequired = Error.Validation(
        ComponentSlug + "ValidationTextBodyIsRequired",
        "Tělo emailu je povinné pole."
    );
    
    public static Error ValidationCodeIsNotRecognizedInListOfTemplates = Error.Validation(
        ComponentSlug + "ValidationCodeIsNotRecognizedInListOfTemplates",
        "Kód není specifikován v listu šablon pro emaily."
    );
}