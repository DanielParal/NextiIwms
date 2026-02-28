using System.Diagnostics;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.Printers.Queries;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Application.Printers.Queries.GetPrinterByCode;
using Nexticz.Module.Sign.DocumentManager.Application.PrintingHandling;
using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Application.Printers.Commands.PrintDocuments;

internal class PrintDocumentsCommandHandler(
    ISender sender,
    ILogger<PrintDocumentsCommandHandler> logger,
    IDocumentManagerPrintHandler printHandler,
    IDocumentManagerUnitOfWork unitOfWork) : IRequestHandler<PrintDocumentsCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(PrintDocumentsCommand request, CancellationToken cancellationToken)
    {
        var printerContractFromSettings =
            await sender.Send(new GetPrinterContractByCodeRequest(request.PrinterCode), cancellationToken);
        if (printerContractFromSettings.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - printer with code: {PrinterCode} was not found. We cannot print documents.",
                request.PrinterCode);
            return PrinterErrors.ValidationPrinterDoesNotExist;
        }

        var printStopwatch  = new Stopwatch();
        printStopwatch.Start();
        
        var printingResult = 
            await printHandler.PrintDocumentsAsync(
                printerContractFromSettings.Value.Ip, 
                request.DocumentsWithPrintCopiesCount,
                cancellationToken);

        var printer = await sender.Send(new GetPrinterByCodeQuery(request.PrinterCode), cancellationToken);
        var existingPrinter = printer.IsError ? new Printer(printerContractFromSettings.Value.Code) : printer.Value;
        if (printer.IsError)
        {
            logger.LogInformation(
                "SIGN - DocumentManager - new printer created. Printer Id: {Id}, printer Code: {PrinterCode}.",
                existingPrinter.Id, existingPrinter.Code);
            
            var printerCreatedEvent = new PrinterCreatedEvent(existingPrinter.Id, existingPrinter.Code, printerContractFromSettings.Value.Ip);
            unitOfWork.StartStream<PrinterCreatedEvent, Printer>(existingPrinter.Id, printerCreatedEvent);
        }
        
        if (printingResult.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - printing failed. Printer: {PrinterCode}, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}.",
                existingPrinter.Code, printingResult.FirstError.Code, printingResult.FirstError.Description);
            
            var printFailedEvent = new DocumentsPrintFailedEvent(
                existingPrinter.Id, existingPrinter.Code, printerContractFromSettings.Value.Ip,
                request.DocumentsWithPrintCopiesCount, printingResult.FirstError.Code, printingResult.FirstError.Description);
            
            unitOfWork.AppendEvent(existingPrinter.Id, printFailedEvent);
            
            return printingResult.Errors;
        }
        
        printStopwatch.Stop();
        var printTimeDuration = printStopwatch.Elapsed;
        var printSucceedEvent = new DocumentsPrintedEvent(
            existingPrinter.Id, existingPrinter.Code, printerContractFromSettings.Value.Ip, request.DocumentsWithPrintCopiesCount, printTimeDuration);
            
        unitOfWork.AppendEvent(existingPrinter.Id, printSucceedEvent);
        
        
        logger.LogInformation(
            "SIGN - DocumentManager - documents printed successfully. Printer Id: {Id}, printer Code: {PrinterCode}, printing time: {PrintingTime} s.",
            existingPrinter.Id, existingPrinter.Code, printTimeDuration.TotalSeconds);
        
        return Result.Success;
    }
}