using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.Settings.Application.EmailTemplates;

internal abstract class EmailTemplateErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-application-emailTemplate-";
    
    public static Error EmailTemplateNotFound = Error.NotFound(
        ComponentSlug + "EmailTemplateNotFound",
        "Emailová šablona nenalezena."
    );
    
    public static Error ValidationEmailTemplateDoesNotExist = Error.Validation(
        ComponentSlug + "ValidationEmailTemplateDoesNotExist",
        "Emailová šablona neexistuje."
    );
    
    public static Error ValidationEmailTemplateWithCodeAlreadyExists = Error.Validation(
        ComponentSlug + "ValidationEmailTemplateWithCodeAlreadyExists",
        "Emailová šablona s tímto kódem již existuje."
    );
}