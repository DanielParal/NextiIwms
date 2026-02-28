using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

internal class MunicipalityDomainErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "municipality-domain-module-";
    
    public static Error ValidationCodeIsRequired() => Error.Validation(
        ComponentSlug + "ValidationCodeIsRequired",
        "Kód je povinné pole."
    );
}