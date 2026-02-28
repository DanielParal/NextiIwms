using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.PrintingPublishers;

internal interface IDocumentManagerPrintingPublisher
{
    Task PublishDocumentsPrintingAsync(Guid signingDeviceId, string signingDeviceCode, string printerCode, DocumentPrintJob[] documentPrintJobs, CancellationToken cancellationToken);
}