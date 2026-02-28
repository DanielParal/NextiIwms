using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SignalRNotifications;
using Nexticz.Module.Sign.DocumentManager.Application.Printers.Commands.PrintDocuments;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.SaveSigningDevicePrintingResult;
using Nexticz.Module.Sign.DocumentManager.Contracts.Printings;
using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.PrintingHandling;

internal class DocumentsPrintingOrchestrator(
    ISender sender, 
    ILogger<DocumentsPrintingOrchestrator> logger,
    IDocumentPrintingNotifier printingNotifier) : IDocumentsPrintingOrchestrator
{
    public async Task<ErrorOr<Success>> OrchestrateAsync(DocumentsPrintingRequested message, bool isLastAttempt, CancellationToken cancellationToken)
    {
        logger.LogInformation("SIGN - DocumentsPrintingOrchestrator - documents printing requested. SigningDeviceId: {SigninDeviceId}, signingDeviceCode: {PrintingDocuments}, " +
                              "printerCode: {PrinterCode}, printDeadline: {PrintDeadline}, loadingDocumentCodes: {LoadingDocumentCodes}",
            message.SigningDeviceId, message.SigningDeviceCode, message.PrinterCode, message.PrintDeadline, message.DocumentPrintJobs.Select(x => x.LoadingDocumentCode));

        var executedAt = DateTimeOffset.UtcNow;
        if (message.PrintDeadline < executedAt)
        {
            logger.LogError("SIGN - DocumentsPrintingOrchestrator - print deadline is expired. This message will not be processed again. SigningDeviceCode: {PrintingDocuments}, printDeadline: {PrintDeadline}",
                message.SigningDeviceCode, message.PrintDeadline);
            return Result.Success;
        }
        
        await printingNotifier.NotifyDocumentsPrintingStartedAsync(message.SigningDeviceCode, cancellationToken);

        var documentPrintJobs = message.DocumentPrintJobs.Select(x => new DocumentPrintJob(x.LoadingDocumentCode, x.DeliveryDocumentCode, x.CopiesCount)).ToArray();
        var printingResult = 
            await sender.Send(
                new PrintDocumentsCommand(
                    message.PrinterCode, 
                    documentPrintJobs),
                cancellationToken);

        await NotifyDeviceWithPrintingResultAsync(printingResult, message.SigningDeviceCode, isLastAttempt, cancellationToken);
        
        await sender.Send(new SaveSigningDevicePrintingResultCommand(message.SigningDeviceId, message.SigningDeviceCode, printingResult, executedAt, documentPrintJobs), cancellationToken);
        
        logger.LogInformation("SIGN - DocumentsPrintingOrchestrator - documents printing finished with result: {@PrintingResult}", printingResult);
        
        return printingResult;
    }

    private async Task NotifyDeviceWithPrintingResultAsync(ErrorOr<Success> result, string signingDeviceCode, bool isLastAttempt, CancellationToken cancellationToken)
    {
        if (result.IsError && isLastAttempt)
        {
            await printingNotifier.NotifyDocumentsPrintingFailedAsync(signingDeviceCode, cancellationToken);
            return;
        }
        
        if (result.IsError && !isLastAttempt)
        {
            await printingNotifier.NotifyDocumentsPrintingFailedButWithRetryAsync(signingDeviceCode, cancellationToken);
            return;
        }
        
        await printingNotifier.NotifyDocumentsPrintingSucceededAsync(signingDeviceCode, cancellationToken);
    }
}