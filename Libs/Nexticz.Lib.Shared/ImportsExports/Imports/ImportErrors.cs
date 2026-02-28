using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Lib.Shared.ImportsExports.Imports;

public abstract class ImportErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "shared-import-";
    
    public static Error ValidationFileHasWrongFormat(string headers) => Error.Validation(
        ComponentSlug + "ValidationFileHasWrongFormat",
        $"Soubor má špatný formát. Musí obsahovat tyto sloupce v přesném pořadí: {headers}"
    );
    
    public static Error ValidationFileHasNoData => Error.Validation(
        ComponentSlug + "validationFileHasNoData",
        "Soubor neobsahuje žádná data."
    );
    
    public static Error ValidationFileIsNotExcel => Error.Validation(
        ComponentSlug + "validationFileIsNotExcel",
        "Soubor není Excel soubor."
    );
    
    public static Error ValidationFileIsNotCsv => Error.Validation(
        ComponentSlug + "validationFileIsNotCsv",
        "Soubor není CSV soubor."
    );
    
    public static Error ValidationFileIsNotProvided => Error.Validation(
        ComponentSlug + "ValidationFileIsNotProvided",
        "Soubor nebyl nalezen."
    );
}