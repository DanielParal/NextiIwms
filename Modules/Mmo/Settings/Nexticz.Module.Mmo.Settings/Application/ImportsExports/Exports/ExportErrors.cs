using ErrorOr;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Domain.ExportEntity;

namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports.Exports;

internal abstract class ExportErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-exportService-";
    
    public static Error ValidationHandlerNotImplementedFor(ExportType exportType) => Error.Validation(
        ComponentSlug + "validationHandlerNotImplementedFor",
        $"Žádný handler nebyl nalezen pro export zdroje: {exportType}."
    );
}