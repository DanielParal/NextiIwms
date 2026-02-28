using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Settings.Application.FileHandling;

internal abstract class FileHandlingErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-fileHandlingService-";
    
    public static Error ValidationFileIsEmpty => Error.Validation(
        ComponentSlug + "ValidationFileIsEmpty",
        "Souboru je prázdný."
    );
    
    public static Error ValidationFileExceedsMaxSize(int maxSizeInMb) => Error.Validation(
        ComponentSlug + "ValidationFileExceedsMaxSize",
        $"Souboru přesáhl maxímální velikost: {maxSizeInMb}MB."
    );
    
    public static Error ValidationMultipartFormRequired => Error.Validation(
        ComponentSlug + "ValidationMultipartFormRequired",
        "Špatný typ requestu. Musíte poslat multipart/form-data."
    );
    
    public static Error ValidationNoFileAttached => Error.Validation(
        ComponentSlug + "ValidationNoFileAttached",
        "Nepřiložili jste žádný soubor."
    );
}