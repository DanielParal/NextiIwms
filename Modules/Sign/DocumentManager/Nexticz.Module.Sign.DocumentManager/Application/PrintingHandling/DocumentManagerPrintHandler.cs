using ErrorOr;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Nexticz.Module.Sign.SharedKernel.FileHandling;
using Nexticz.Lib.Shared.FileHandling.Assets;
using Nexticz.Lib.Shared.PdfUtils;
using Nexticz.Lib.Shared.PrintingUtils;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.PrintingHandling;

internal class DocumentManagerPrintHandler(
    ILogger<DocumentManagerPrintHandler> logger,
    AssetsSettings assetsSettings,
    IFeatureManager featureManager,
    IClock clock) : PrintHandler(logger), IDocumentManagerPrintHandler
{
    private const string SignShouldPrintDocumentsInPrinters = nameof(SignShouldPrintDocumentsInPrinters);
    
    public async Task<ErrorOr<Success>> PrintDocumentsAsync(
        string printerIp,
        DocumentPrintJob[] documentsToPrint,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(printerIp))
                return PrintingErrors.ValidationPrinterIpIsRequired;
            
            var documentPaths = new List<string>();
            foreach (var documentToPrint in documentsToPrint)
            {
                if (documentToPrint.CopiesCount == 0)
                    continue;

                var filePath = documentToPrint.DeliveryDocumentCode is null
                    ? DirectoryNamesProvider.GetHistoryLoadingDocumentFilePath(assetsSettings, documentToPrint.LoadingDocumentCode)
                    : DirectoryNamesProvider.GetHistoryDeliveryDocumentFilePath(assetsSettings, documentToPrint.LoadingDocumentCode, documentToPrint.DeliveryDocumentCode);

                for (var i = 0; i < documentToPrint.CopiesCount; i++)
                {
                    documentPaths.Add(filePath);
                }
            }

            if (documentPaths.Count == 0)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - there is nothing to print. There are no document paths for print. Copies count is most likely 0 for all documents. Printer: {PrinterIp}",
                    printerIp);
                return Result.Success;
            }
            
            var finalDocumentToPrint = PdfMerger.MergePdfsAndNormalizedToPortraitForPrinting(documentPaths.ToArray(), true);

            if (await featureManager.IsEnabledAsync(nameof(SignShouldPrintDocumentsInPrinters)))
                return await PrintAsync(printerIp, finalDocumentToPrint, 1, true, null, null,
                    cancellationToken: cancellationToken);

            var printingsFolder = DirectoryNamesProvider.GetHistoryPrintingsFolder(assetsSettings);
            Directory.CreateDirectory(printingsFolder);
            await File.WriteAllBytesAsync(
                Path.Combine(printingsFolder, $"Printing_{clock.TenantNowOffset:yyyy-MM-dd_hhmmss}.pdf"),
                finalDocumentToPrint, cancellationToken);
            return Result.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(
                "SIGN - DocumentManager - there was an unexpected error when printing on printer: {PrinterIp}. We cannot print documents. Error message: {ErrorMessage}.",
                printerIp, ex.Message);
            return PrintingErrors.UnexpectedErrorDuringPrinting(ex.Message);
        }
    }
}