using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Auth.Domain.UserAggregate;

internal abstract class UserDomainErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "auth-module-users-domain-";
    
    public static Error ValidationUserNameMustBeFilledIn => Error.Validation(
        ComponentSlug + "ValidationUserNameMustBeFilledIn",
        "Uživatelské jméno musí být vyplněno.");
    
    public static Error ValidationEmailMustBeFilledIn => Error.Validation(
        ComponentSlug + "ValidationEmailMustBeFilledIn",
        "Email musí být vyplněn.");
    
    public static Error ValidationInvalidEmailFormat => Error.Validation(
        ComponentSlug + "ValidationInvalidEmailFormat",
        "Formát emailu není validní.");
}