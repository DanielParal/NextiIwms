using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.PrintingHandling;

internal interface IDocumentManagerPrintHandler
{
    Task<ErrorOr<Success>> PrintDocumentsAsync(
        string printerIp,
        DocumentPrintJob[] documentsToPrint,
        CancellationToken cancellationToken);
}