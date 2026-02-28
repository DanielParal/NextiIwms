using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Lib.Shared.FileHandling;

internal abstract class FileHandlingErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "magic-shared-fileHandlingService-";
    
    public static Error ValidationFileIsEmpty => Error.Validation(
        ComponentSlug + "ValidationFileIsEmpty",
        "Souboru je prázdný."
    );
    
    public static Error ValidationFileExceedsMaxSize(int maxSizeInKb) => Error.Validation(
        ComponentSlug + "ValidationFileExceedsMaxSize",
        $"Souboru přesáhl maxímální velikost: {maxSizeInKb}KB."
    );
    
    public static Error ValidationInvalidFileType(string allowedExtensions) => Error.Validation(
        ComponentSlug + "validationInvalidFileType",
        $"Souboru má nepovolený typ. Povoletné typy: {allowedExtensions}."
    );
    
    public static Error ValidationMultipartFormRequired => Error.Validation(
        ComponentSlug + "ValidationMultipartFormRequired",
        "Špatný typ requestu. Musíte poslat multipart/form-data."
    );
    
    public static Error ValidationNoFileAttached => Error.Validation(
        ComponentSlug + "ValidationNoFileAttached",
        "Nepřiložili jste žádný soubor."
    );
    
    public static Error ValidationNoFilesProvidedToCreateZip => Error.Validation(
        ComponentSlug + "ValidationNoFilesProvidedToCreateZip",
        "Nebyli nalezeny žádné soubory k vytvoření ZIP souboru."
    );
    
    public static Error ZipCreationFailed => Error.Validation(
        ComponentSlug + "ZipCreationFailed",
        "Něco se pokazilo při vytváření zip souboru."
    );
    
    public static Error ValidationInvalidZipFileName => Error.Validation(
        ComponentSlug + "ValidationInvalidZipFileName",
        "Jméno zip souboru je povinné pole."
    );
}