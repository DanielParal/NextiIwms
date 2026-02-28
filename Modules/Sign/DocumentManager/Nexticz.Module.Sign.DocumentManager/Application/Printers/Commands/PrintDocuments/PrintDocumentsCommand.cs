using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.Printers.Commands.PrintDocuments;

internal record PrintDocumentsCommand(
    string PrinterCode,
    DocumentPrintJob[] DocumentsWithPrintCopiesCount) : IDocumentManagerCommand<ErrorOr<Success>>;