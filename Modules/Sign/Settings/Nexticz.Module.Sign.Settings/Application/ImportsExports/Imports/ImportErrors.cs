using ErrorOr;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Domain.ImportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.ImportsExports.Imports;

internal abstract class ImportErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-api-importService-";
    
    public static Error ValidationFileIsNotExcel => Error.Validation(
        ComponentSlug + "validationFileIsNotExcel",
        "Soubor není Excel soubor."
    );
    
    public static Error ValidationFileIsNotProvided => Error.Validation(
        ComponentSlug + "ValidationFileIsNotProvided",
        "Soubor nebyl nalezen."
    );
    
    public static Error ValidationHandlerNotImplementedFor(ImportType importType) => Error.Validation(
        ComponentSlug + "ValidationHandlerNotImplementedFor",
        $"Žádný handler nebyl nalezen pro import zdroje: {importType}."
    );
    
    public static Error ValidationFormFileIsRequired => Error.Validation(
        ComponentSlug + "ValidationFormFileIsRequired",
        "Soubor je povinné pole."
    );
}