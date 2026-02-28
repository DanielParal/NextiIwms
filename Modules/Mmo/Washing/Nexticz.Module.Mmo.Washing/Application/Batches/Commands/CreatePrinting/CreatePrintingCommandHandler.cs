using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Nexticz.Lib.Shared.PdfUtils.Models;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchByKitId;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Application.Printings;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;
using Nexticz.Module.Mmo.Washing.Domain.KitWashCycleEntity;
using Nexticz.Module.Mmo.Washing.Domain.PrintingEntity;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.CreatePrinting;

internal class CreatePrintingCommandHandler(
    ISender sender,
    IWashingPrintHandler washingPrintHandler,
    IClock clock,
    IWashingUnitOfWork unitOfWork,
    ILogger<CreatePrintingCommandHandler> logger,
    BarcodeGenerator barcodeGenerator)
    : IRequestHandler<CreatePrintingCommand, ErrorOr<CreatePrintingCommandResponse>>
{
    public async Task<ErrorOr<CreatePrintingCommandResponse>> Handle(CreatePrintingCommand request,
        CancellationToken cancellationToken)
    {
        var batch = await sender.Send(new GetBatchByKitIdQuery(request.KitId), cancellationToken);
        if (batch.IsError)
        {
            logger.LogWarning("Washing Printing - Cannot print document because kit does not exist. KitId: {KitId}",
                request.KitId);
            return batch.Errors;
        }

        var kit = batch.Value.KitWashCycles.First(x => x.Id == request.KitId);

        var printerSettings = await sender.Send(new GetPrinterSettingsContractByLineCodeQuery(request.LineCode),
            cancellationToken);
        if (printerSettings.IsError)
        {
            logger.LogWarning(
                "Washing Printing - Cannot print document because we cannot get printer settings for kit. KitId: {KitId}, lineCode: {LineCode}, errorMessage: {ErrorMessage}.",
                request.KitId, request.LineCode, printerSettings.FirstError.Description);
            return printerSettings.Errors;
        }

        var pageSize = printerSettings.Value.PageSize is null ? PrinterPageSize.A4 : (PrinterPageSize)printerSettings.Value.PageSize;
        var data = GetPrintingData(batch.Value, kit, pageSize);
        var isPdfOnly = string.IsNullOrWhiteSpace(printerSettings.Value.Ip);
        var pdf = await KitPrintingBuilder.BuildKitPrintingPdfAsync(
            $"{kit.GlobalKitsCount!.ToString()}.pdf",
            data,
            pageSize);

        ErrorOr<Success> printingResult = Result.Success;
        if (!isPdfOnly)
            printingResult =
                await washingPrintHandler.PrintAsync(
                    printerSettings.Value.Ip!, pdf.ContentBytes, printerSettings.Value.CopyCount ?? 1, false,
                    printerSettings.Value.UserName, printerSettings.Value.Password, cancellationToken);

        var printing =
            new Printing(
                batch.Value.Id,
                kit.Id,
                clock.UtcNowOffset,
                printingResult.IsError ? PrintingStatus.Failed : PrintingStatus.Success,
                isPdfOnly ? PrintingType.Download : PrintingType.Printout,
                printingResult.IsError ? printingResult.FirstError.Description : null);

        var printingCreatedEvent =
            new PrintingCreatedEvent(printing.Id, printing.BatchId, printing.KitId,
                printing.DatePrinted, printing.Status, printing.Type, printing.FailureReason);

        unitOfWork.AppendEvent(printing.BatchId, printingCreatedEvent);

        logger.LogInformation("Washing Printing - Document printed successfully for kit. KitId: {KitId}",
            request.KitId);
        return new CreatePrintingCommandResponse(printing, isPdfOnly ? pdf : null);
    }

    private Dictionary<string, string> GetPrintingData(Batch batch, KitWashCycle kitWashCycle, PrinterPageSize pageSize)
    {
        var data = new Dictionary<string, string>();
        var completedKitsCount = kitWashCycle.GlobalKitsCount!.ToString()!.PadLeft(8, '0');
        var qrCode = QrGenerator.GetQrCode(completedKitsCount);

        data.Add("CompletedKitsCountQrCode", qrCode);
        data.Add("DefiningPackagingCode", batch.DefiningPackagingCode);
        data.Add("UniqueKitCode", batch.KitCode);

        data.Add("DatePrinted", kitWashCycle.EndDate.ToString("dd.MM.yyyy"));
        data.Add("DatePrintedTime", kitWashCycle.EndDate.ToString("HH:mm"));
        data.Add("WorkerName", kitWashCycle.WorkerName!);

        data.Add("KitSapDefinitionName", batch.KitSapDefinitionName);

        data.Add("SapBarcodeDivHeight", pageSize == PrinterPageSize.A4 ? "200px" : "100px");
        if (string.IsNullOrWhiteSpace(batch.SapBarcode))
        {
            data.Add("IsSapBarcodeVisible", "hidden");
        }
        else
        {
            var barcodeImage = barcodeGenerator.GetBarcode(batch.SapBarcode);
            data.Add("SapBarcode", batch.SapBarcode);
            data.Add("SapBarcodeImage", barcodeImage ?? string.Empty);
            data.Add("IsSapBarcodeVisible", "visible");
        }


        var splitCompletedKitsCount = SplitValue(completedKitsCount, 2);
        data.Add("CompletedKitsCountPrefix", splitCompletedKitsCount.Prefix);
        data.Add("CompletedKitsCountLastTwoDigits", splitCompletedKitsCount.Suffix);

        var splitKitNumber = SplitValue(batch.KitNumber, 4);
        data.Add("CustomerNumberPrefix", splitKitNumber.Prefix);
        data.Add("CustomerNumberLast4Digits", splitKitNumber.Suffix);

        return data;
    }

    private static (string Prefix, string Suffix) SplitValue(string value, int suffixLength)
    {
        return (value[..^suffixLength], value[^suffixLength..]);
    }
}