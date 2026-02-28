using ClosedXML.Excel;
using MediatR;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.ImportsExports.Imports;
using Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Commands.CreateDeliveryMethod;
using Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Queries.GetDeliveryMethodByCode;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Imports.ImportHandlers;
using Nexticz.Module.Sign.Settings.Domain.ImportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.ImportExport;

internal class DeliveryMethodImportXlsxHandler(ISender sender) : 
    ImportXlsxBaseTemplateHandler<ImportType>, ISettingsImportHandler<ImportType>
{
    protected override ImportType ImportType { get; } = ImportType.DeliveryMethodXlsx;
    protected override string[] ExpectedFileHeader { get; } = DeliveryMethodHeader.ExpectedFileHeader;
    
    protected override Task<(List<object> Items, List<ImportBaseError> Errors)> GetItemsFromExcelAsync(XLWorkbook workbook, CancellationToken cancellationToken)
    {
        var items = new List<object>();
        var errors = new List<ImportBaseError>();
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RowsUsed();

        foreach (var row in rows.Skip(1)) // Skip the header row (row 1)
        {
            var code = row.Cell(DeliveryMethodHeader.Kod).GetTrimString().ToUpperInvariant();
            var name = row.Cell(DeliveryMethodHeader.Jmeno).GetTrimString();
            row.Cell(DeliveryMethodHeader.PocetKopiiProNakladniList).TryGetIntValue(out var loadingDocumentPrintCopies);
            row.Cell(DeliveryMethodHeader.PocetKopiiProDodaciList).TryGetIntValue(out var deliveryDocumentPrintCopies);

            if (string.IsNullOrWhiteSpace(code))
            {
                errors.Add(new ImportBaseError($"{code}", $"Číslo řádku: {row.RowNumber()}. Nulová hodnota pro kód."));
                continue;
            }
            
            items.Add(new DeliveryMethodItem(code, name, loadingDocumentPrintCopies, deliveryDocumentPrintCopies, row.RowNumber()));
        }
        
        return Task.FromResult((items, errors));
    }

    protected override async Task<(ImportBaseItem? SuccessItem, ImportBaseError? Error)> ProcessItemAsync(object item, CancellationToken cancellationToken)
    {
        if (item is not DeliveryMethodItem deliveryMethodItem)
            return (null, new ImportBaseError("", "Nesprávný typ položky při zpracování importu."));

        var existingDeliveryMethod = await sender.Send(new GetDeliveryMethodByCodeQuery(deliveryMethodItem.Code), cancellationToken);

        if (existingDeliveryMethod.HasValue())
            return (null, new ImportBaseError($"{deliveryMethodItem.Code}", 
                $"Číslo řádku: {deliveryMethodItem.RowNumber}. Partner s tímto kódem neexistuje."));
        
        var createResult = 
            await sender.Send(new CreateDeliveryMethodCommand(
                deliveryMethodItem.Code, deliveryMethodItem.Name, 
                deliveryMethodItem.LoadingDocumentPrintCopiesCount, deliveryMethodItem.DeliveryDocumentPrintCopiesCount), 
                cancellationToken);
        
        return createResult.IsError ? 
            (null, new ImportBaseError($"{deliveryMethodItem.Code}", 
                $"Číslo řádku: {deliveryMethodItem.RowNumber}. {createResult.FirstError.Description}")) : 
            (new ImportBaseItem(createResult.Value.Id, $"{createResult.Value.Code}"), null);
    }
}