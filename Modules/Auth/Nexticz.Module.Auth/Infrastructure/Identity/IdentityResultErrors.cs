using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Auth.Infrastructure.Identity;

public abstract class IdentityResultErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "auth-identityResult-infrastructure-";
    
    public static Error ErrorDeletingUser => Error.Validation(
        ComponentSlug + "ErrorDeletingUser",
        "Error při mazání uživatele.");
    
    public static Error ErrorUpdatingUser => Error.Validation(
        ComponentSlug + "ErrorUpdatingUser",
        "Error při upravování uživatele.");
    
    public static Error InvalidPassword => Error.Validation(
        ComponentSlug + "InvalidPassword",
        ".");
}