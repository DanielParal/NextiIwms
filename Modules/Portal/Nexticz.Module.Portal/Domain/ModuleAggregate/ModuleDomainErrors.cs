using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Portal.Domain.ModuleAggregate;

internal class ModuleDomainErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "portal-domain-module-";
    
    public static Error ValidationNameIsRequired() => Error.Validation(
        ComponentSlug + "ValidationNameIsRequired",
        "Jméno je povinné pole."
    );
    
    public static Error ValidationIconIsRequired() => Error.Validation(
        ComponentSlug + "ValidationIconIsRequired",
        "Ikona je povinné pole."
    );
    
    public static Error ValidationBaseUrlIsRequired() => Error.Validation(
        ComponentSlug + "ValidationBaseUrlIsRequired",
        "Url je povinné pole."
    );
}