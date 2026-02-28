using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

internal class AddressLocationDomainErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "addressLocation-domain-module-";
    
    public static Error ValidationAdmCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationAdmCodeIsRequired",
        "Adm kód je povinné pole."
    );
    
    public static Error ValidationMunicipalityCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationMunicipalityCodeIsRequired",
        "Kód správní jednotky je povinné pole."
    );
}