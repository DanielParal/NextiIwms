using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.DocumentManager.Application.PrintingHandling;

internal abstract class PrintingErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-documentManager-api-printingService-";
    
    public static Error ValidationPrinterIpIsRequired => Error.Validation(
        ComponentSlug + "ValidationPrinterIpIsRequired",
        "IP tiskárny není vyplněna."
    );
    
    public static Error UnexpectedErrorDuringPrinting(string errorMessage) => Error.Validation(
        ComponentSlug + "UnexpectedErrorDuringPrinting",
        $"Nastala neočekávaná chyba při tisku: {errorMessage}."
    );
}