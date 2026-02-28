using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.EmailSender.Application.EmailMessages;

internal abstract class EmailMessageErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "emailSender-service-emailMessage-";
    
    public static Error EmailMessageNotFound = Error.NotFound(
        ComponentSlug + "EmailMessageNotFound",
        "Email nebyl nazen."
    );
    
    public static Error ValidationEmailMessageDoesNotExist = Error.Validation(
        ComponentSlug + "ValidationEmailMessageDoesNotExist",
        "Email s tímto id neexistuje."
    );
}