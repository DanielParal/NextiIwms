using ErrorOr;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;

namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports;

internal abstract class ImportErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-importService-";
    
    public static Error ValidationHandlerNotImplementedFor(ImportType importType) => Error.Validation(
        ComponentSlug + "validationHandlerNotImplementedFor",
        $"Žádný handler nebyl nalezen pro import zdroje: {importType}."
    );
    
    public static Error ValidationFormFileIsRequired => Error.Validation(
        ComponentSlug + "validationFormFileIsRequired",
        "Soubor je povinné pole."
    );
    
    public static Error ValidationFileIsNotExcel => Error.Validation(
        ComponentSlug + "validationFileIsNotExcel",
        "Soubor není Excel soubor."
    );
    
    public static Error ValidationMultipartFormRequired => Error.Validation(
        ComponentSlug + "ValidationMultipartFormRequired",
        "Špatný typ requestu. Musíte poslat multipart/form-data."
    );
    
    public static Error ValidationNoFileAttached => Error.Validation(
        ComponentSlug + "ValidationNoFileAttached",
        "Nepřiložili jste žádný soubor."
    );
    
    public static Error ValidationMissingImportSourceType => Error.Validation(
        ComponentSlug + "ValidationMissingImportSourceType",
        "Chybí typ zdroje importu."
    );
}