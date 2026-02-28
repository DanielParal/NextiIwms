using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Lib.Shared.PrintingUtils;

internal abstract class PrintingErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "shared-printingUtils-service-";
    
    public static Error ValidationUnExpectedError(string errorMessage) => Error.Validation(
        ComponentSlug + "ValidationUnExpectedError",
        $"Nastala neočekávaná chyba při tisku: {errorMessage}."
    );
    
    public static Error ValidationPrintNotSuccessful(string reasons) => Error.Validation(
        ComponentSlug + "ValidationUnExpectedError",
        $"Tisk se nepovedl z tohoto důvodu: {reasons}."
    );
}