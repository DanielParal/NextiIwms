using ErrorOr;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Domain.ExportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.ImportsExports.Exports;

internal abstract class ExportErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-api-exportService-";
    
    public static Error ValidationHandlerNotImplementedFor(ExportType exportType) => Error.Validation(
        ComponentSlug + "validationHandlerNotImplementedFor",
        $"Žádný handler nebyl nalezen pro export zdroje: {exportType}."
    );
}