using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Cuzk.Application.FileHandling;

internal abstract class FileHandlingErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "file-handling-application-modulesService-";
    
    public static Error ErrorSavingRequestedFile(string correlationId) => Error.Failure(
        ComponentSlug + "ErrorSavingRequestedFile",
        $"Error při ukládání souboru pro import. Kontaktujte podporu s tímto id: {correlationId}."
    );
}