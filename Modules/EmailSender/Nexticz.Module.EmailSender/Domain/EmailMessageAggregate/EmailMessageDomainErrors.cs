using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;

internal abstract class EmailMessageDomainErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "emailSender-domain-emailMessage-";
    
    public static Error ValidationAtLeastOneEmailIsRequired = Error.Validation(
        ComponentSlug + "ValidationAtLeastOneEmailIsRequired",
        "Musíte zadat alespoň jeden email."
    );
    
    public static Error ValidationInitiatorIsNotCorrectlySet = Error.Validation(
        ComponentSlug + "ValidationInitiatorIsNotCorrectlySet",
        "Původce zprávy není správně nastaven."
    );
}