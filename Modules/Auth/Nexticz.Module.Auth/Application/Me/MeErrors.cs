using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Auth.Application.Me;

internal abstract class MeErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "auth-module-me-application-";
    
    public static Error ValidationOldPasswordIsIncorrect => Error.Validation(
        ComponentSlug + "ValidationOldPasswordIsIncorrect",
        "Vaše heslo je nesprávné");
}