using ErrorOr;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.ImportsExports.Imports;

internal abstract class ImportErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "cuzk-api-importService-";
    
    public static Error ValidationHandlerNotImplementedFor(ImportType importType) => Error.Validation(
        ComponentSlug + "ValidationHandlerNotImplementedFor",
        $"Žádný handler nebyl nalezen pro import zdroje: {importType}."
    );
    
    public static Error ValidationFileIsNotFound => Error.Validation(
        ComponentSlug + "ValidationFileIsNotFound",
        "Nebyl nalezen soubor."
    );
}