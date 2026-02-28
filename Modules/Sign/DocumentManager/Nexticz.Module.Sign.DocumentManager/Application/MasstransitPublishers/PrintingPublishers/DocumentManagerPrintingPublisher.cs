using Nexticz.Module.Sign.DocumentManager.Contracts.Printings;
using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.PrintingPublishers;

internal class DocumentManagerPrintingPublisher(IDocumentManagerPublisher messagePublisher) : IDocumentManagerPrintingPublisher
{
    public async Task PublishDocumentsPrintingAsync(Guid signingDeviceId, string signingDeviceCode, string printerCode, DocumentPrintJob[] documentPrintJobs,
        CancellationToken cancellationToken)
    {
        var printDeadline = DateTimeOffset.UtcNow.AddMinutes(10);
        var documentsPrintingRequestedMessage = new DocumentsPrintingRequested(
            signingDeviceId,
            signingDeviceCode,
            printDeadline,
            printerCode,
            documentPrintJobs.Select(x => new DocumentPrintJobContract(x.LoadingDocumentCode, x.DeliveryDocumentCode, x.CopiesCount)).ToArray());
        await messagePublisher.PublishAsync(documentsPrintingRequestedMessage, cancellationToken);
    }
}