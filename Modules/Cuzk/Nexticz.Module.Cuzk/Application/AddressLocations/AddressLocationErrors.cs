using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Cuzk.Application.AddressLocations;

internal abstract class AddressLocationErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "addressLocation-application-service-";
    
    public static Error ValidationAddressLocationWithAdmCodeAlreadyExists => Error.Validation(
        ComponentSlug + "ValidationAddressLocationWithAdmCodeAlreadyExists",
        "Adresní místo s tímto ADM kódem již existuje."
    );
    
    public static Error ValidationAddressLocationWithAdmCodeDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationAddressLocationWithAdmCodeDoesNotExist",
        "Adresní místo s tímto ADM kódem již neexistuje."
    );
    
    public static Error ValidationAdmCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationAdmCodeIsRequired",
        "ADM kód je povinné pole."
    );
}