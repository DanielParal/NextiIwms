using ErrorOr;

namespace Nexticz.Lib.Shared.Errors;

public static class Errors 
{
    public class Common : IErrorComponentSlugProvider
    {
        public static string ComponentSlug { get; } = "shared-api-common-";

        public static Error BadRequest => Error.Failure(
            code: ComponentSlug + "badRequest",
            description: "Špatný požadavek");
        public static Error Unauthorize => Error.Unauthorized(
            code: ComponentSlug + "unauthorize",
            description: "Přístup odepřen");
        public static Error InternalServerError => Error.Unexpected(
            code: ComponentSlug + "internalServerError",
            description: "Neočekávaná interní chyba");
        public static Error CanNotSendEmail => Error.Failure(
            code: ComponentSlug + "canNotSendEmail",
            description: "Nepodařilo se odeslat e-mail");
    }
}