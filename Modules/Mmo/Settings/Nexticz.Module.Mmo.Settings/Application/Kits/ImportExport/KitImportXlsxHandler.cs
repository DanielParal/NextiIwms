using ClosedXML.Excel;
using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.ImportsExports.Imports;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports.ImportHandlers;
using Nexticz.Module.Mmo.Settings.Application.Kits.Commands.CreateKit;
using Nexticz.Module.Mmo.Settings.Application.Kits.Commands.UpdateKit;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitByCode;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.ImportExport;

internal class KitImportXlsxHandler(
    ISender sender) 
    : ImportXlsxBaseTemplateHandler<ImportType>, ISettingsImportHandler<ImportType>
{
    protected override string[] ExpectedFileHeader { get; } = KitHeader.ExpectedFileHeader;
    protected override ImportType ImportType { get; } = ImportType.KitXlsx;
    
    protected override Task<(List<object> Items, List<ImportBaseError> Errors)> GetItemsFromExcelAsync(XLWorkbook workbook, CancellationToken cancellationToken)
    {
        var items = new List<KitItem>();
        var errors = new List<ImportBaseError>();
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RowsUsed();

        foreach (var row in rows.Skip(1)) // Skip the header row (row 1)
        {
            var kitCode = GetKitCode(row);
            if (IsInErrors(kitCode, errors))
                continue;
            
            var existingKit = items.FirstOrDefault(x => x.KitCode == kitCode);
            
            var itemResult = GetItemResult(row);
            
            if (itemResult.Item is null)
            {
                errors.Add(itemResult.Error!);
                continue;
            }

            if (existingKit is not null &&
                existingKit.PackagingCodeQuantities.Any(x => x.Code == itemResult.Item!.PackagingCodeQuantities[0].Code))
            {
                errors.Add(new ImportBaseError(kitCode, $"Číslo řádku: {row.RowNumber()}. Kód balení {itemResult.Item!.PackagingCodeQuantities[0].Code} je v kitu obsažen dvakrát."));
                continue; 
            }

            if (existingKit is not null)
            {
                existingKit.AddPackagingCodeQuantity(itemResult.Item!.PackagingCodeQuantities[0].Code, itemResult.Item!.PackagingCodeQuantities[0].Quantity);
                continue;
            }
            
            items.Add(itemResult.Item!);
        }
        
        RemoveErrorKitCodesFromExistingItems(items, errors);
        
        return Task.FromResult((items.Cast<object>().ToList(), errors));
    }
    
    protected override async Task<(ImportBaseItem? SuccessItem, ImportBaseError? Error)> ProcessItemAsync(
        object item, 
        CancellationToken cancellationToken)
    {
        if (item is not KitItem kitItem)
            return (null, new ImportBaseError("", "Nesprávný typ položky při zpracování importu."));
        
        var existingKit = await sender.Send(new GetKitByCodeQuery(kitItem.KitCode), cancellationToken);

        if (existingKit.IsError)
        {
            var createResult = await CreateKitAsync(kitItem, cancellationToken);
            return createResult.IsError ? 
                (null, new ImportBaseError(kitItem.KitCode, $"Číslo řádku: {kitItem.RowNumber}. {createResult.FirstError.Description}")) : 
                (new ImportBaseItem(createResult.Value.Id, createResult.Value.Code), null);
        }
        
        var updateResult = await UpdateKitAsync(kitItem, cancellationToken);
        return updateResult.IsError ? 
            (null, new ImportBaseError(kitItem.KitCode, $"Číslo řádku: {kitItem.RowNumber}. {updateResult.FirstError.Description}")) : 
            (new ImportBaseItem(existingKit.Value.Id, existingKit.Value.Code), null);  
    }
    
    private async Task<ErrorOr<Kit>> CreateKitAsync(
        KitItem kitItem, 
        CancellationToken cancellationToken)
    {
        var createCommand = new CreateKitCommand(
            new CreateKitRequest(
                kitItem.KitTypeCode,
                kitItem.KitSapDefinitionCode,
                kitItem.DepositorCode,
                kitItem.ManufactureCode,
                kitItem.KitNumber,
                kitItem.Note,
                kitItem.DefiningPackagingCode,
                kitItem.DryingTime,
                kitItem.PackagingCodeQuantities
                    .Select(x => new PackagingQuantityContract(x.Code, string.Empty, x.Quantity))
                    .ToArray(),
                [])
                );

        return await sender.Send(createCommand, cancellationToken);
    }

    private async Task<ErrorOr<Updated>> UpdateKitAsync(
        KitItem kitItem,
        CancellationToken cancellationToken)
    {
        var updateCommand = new UpdateKitCommand(
            kitItem.KitCode,
            new UpdateKitRequest(
                kitItem.ManufactureCode,
                kitItem.KitSapDefinitionCode,
                kitItem.Note,
                kitItem.DefiningPackagingCode,
                kitItem.DryingTime,
                kitItem.PackagingCodeQuantities
                    .Select(x => new PackagingQuantityContract(x.Code, string.Empty, x.Quantity))
                    .ToArray(),
                [])
        );

        return await sender.Send(updateCommand, cancellationToken);
    }

    private static bool IsInErrors(string kitNumber, List<ImportBaseError> errors)
    {
        return errors.FirstOrDefault(x => x.Code == kitNumber) is not null;
    }
    
    private static string GetKitCode(IXLRow row)
    {
        var kitNumber = row.Cell(KitHeader.CisloKitu).GetTrimString().ToUpperInvariant();
        var depositorCode = row.Cell(KitHeader.KodUkladatele).GetTrimString().ToUpperInvariant();
        var kitTypeCode = row.Cell(KitHeader.KodTypuKitu).GetTrimString().ToUpperInvariant();
        return depositorCode + kitTypeCode + kitNumber;
    }

    private static (KitItem? Item, ImportBaseError? Error) GetItemResult(IXLRow row)
    {
        if (!row.Cell(KitHeader.CasChladnuti).TryGetIntValue(out var dryingTime))
        {
            var error = new ImportBaseError(GetKitCode(row), $"Číslo řádku: {row.RowNumber()}. Neplatná hodnota pro sloupec: {nameof(KitHeader.CasChladnuti)}");
            return (null, error);
        }
            
        if (!row.Cell(KitHeader.PocetBaleniVKitu).TryGetIntValue(out var packagingCodeQuantity))
        {
            var error = new ImportBaseError(GetKitCode(row), $"Číslo řádku: {row.RowNumber()}. Neplatná hodnota pro sloupec: {nameof(KitHeader.PocetBaleniVKitu)}");
            return (null, error);
        }

        var item = new KitItem(
            kitNumber: row.Cell(KitHeader.CisloKitu).GetTrimString(),
            depositorCode: row.Cell(KitHeader.KodUkladatele).GetTrimString(),
            kitTypeCode: row.Cell(KitHeader.KodTypuKitu).GetTrimString(),
            kitSapDefinitionCode: row.Cell(KitHeader.KodDefiniceSapuKitu).GetTrimString(),
            manufactureCode: row.Cell(KitHeader.KodVyroby).GetTrimString(),
            definingPackagingCode: row.Cell(KitHeader.KodUrcujicihoBaleni).GetTrimString(),
            note: row.Cell(KitHeader.Poznamka).GetTrimString(),
            dryingTime: dryingTime,
            hasKitInstructionFile: ConvertHasKitInstructionFile(row.Cell(KitHeader.ObsahujeBaliciPredpis).GetTrimString()),
            rowNumber: row.RowNumber());
            
        var packagingCode = row.Cell(KitHeader.KodBaleni).GetTrimString();
        item.AddPackagingCodeQuantity(packagingCode, packagingCodeQuantity);
        
        return (item, null);
    }

    private static void RemoveErrorKitCodesFromExistingItems(List<KitItem> items, List<ImportBaseError> errors)
    {
        var errorCodes = errors.Select(x => x.Code).ToHashSet();
        items.RemoveAll(x => errorCodes.Contains(x.KitCode));
    }
    
    private static bool ConvertHasKitInstructionFile(string value)
    {
        return value.Trim().ToLower() switch
        {
            "ne" => false,
            "false" => false,
            "0" => false,
            _ => true
        };
    }
}