using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Auth.Application.ApiKeys;

internal abstract class ApiKeyErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "auth-module-apikeys-application-";
    
    public static Error ValidationUserWithIdDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationUserWithIdDoesNotExist",
        "Uživatel s tímto Id neexistuje");
}