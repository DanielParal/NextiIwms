using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Auth.Domain.AppUserApiKeys;

public abstract class ApiKeyErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "auth-api-apiKeyService-";

    public static Error ApiKeyNotExist => Error.Validation(
        code: ComponentSlug + "apiKeyNotExist",
        description: "Api key neexistuje");
    public static Error ApiKeyUserNotExist => Error.Validation(
        code: ComponentSlug + "apiKeyUserNotExist",
        description: "Uživatel pro Api key neexistuje");
}