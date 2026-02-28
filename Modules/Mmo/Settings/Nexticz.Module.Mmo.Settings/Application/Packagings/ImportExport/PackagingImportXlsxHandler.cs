using ClosedXML.Excel;
using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.ImportsExports.Imports;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports.ImportHandlers;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Commands.CreatePackaging;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Commands.UpdatePackaging;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingByCode;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachines;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.ImportExport;

internal class PackagingImportXlsxHandler(
    ISender sender) 
    : ImportXlsxBaseTemplateHandler<ImportType>, ISettingsImportHandler<ImportType>
{
    protected override string[] ExpectedFileHeader { get; } = PackagingHeader.ExpectedFileHeader;
    
    protected override ImportType ImportType { get; } = ImportType.PackagingXlsx;

    protected override async Task<(List<object> Items, List<ImportBaseError> Errors)> GetItemsFromExcelAsync(XLWorkbook workbook, CancellationToken cancellationToken)
    {
        var items = new List<PackagingItem>();
        var errors = new List<ImportBaseError>();
        
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RowsUsed();
        var washingMachinesFromDb = await sender.Send(new GetWashingMachinesQuery(new BaseFilteringParams()), cancellationToken);
        
        foreach (var row in rows.Skip(1)) // Skip the header row (row 1)
        {
            var itemResult = GetItemResult(row, washingMachinesFromDb.Data);

            if (itemResult.Item is null)
            {
                errors.Add(itemResult.Error!);
                continue;
            }

            if (items.Any(x => x.Code == itemResult.Item.Code))
            {
                errors.Add(new ImportBaseError(itemResult.Item.Code, $"Číslo řádku: {row.RowNumber()}. Balení je v importu obsaženo dvakrát. Zkontrolujte, které hodnoty jsou správné."));
                continue;
            }
            
            items.Add(itemResult.Item);
        }
        
        RemoveErrorPackagingCodesFromExistingItems(items, errors);
        
        return (items.Cast<object>().ToList(), errors);
    }
    
    protected override async Task<(ImportBaseItem? SuccessItem, ImportBaseError? Error)> ProcessItemAsync(
        object item, 
        CancellationToken cancellationToken)
    {
        if (item is not PackagingItem packagingItem)
            return (null, new ImportBaseError("", "Nesprávný typ položky při zpracování importu."));
        
        var existingPackaging = await sender.Send(new GetPackagingByCodeQuery(packagingItem.Code), cancellationToken);

        if (existingPackaging.IsError)
        {
            var createResult = await CreatePackagingAsync(packagingItem, cancellationToken);
            return createResult.IsError ? 
                (null, new ImportBaseError(packagingItem.Code, $"Číslo řádku: {packagingItem.RowNumber}. {createResult.FirstError.Description}")) : 
                (new ImportBaseItem(createResult.Value.Id, createResult.Value.Code), null);
        }
        
        var updateResult = await UpdatePackagingAsync(packagingItem, cancellationToken);
        return updateResult.IsError ? 
            (null, new ImportBaseError(packagingItem.Code, $"Číslo řádku: {packagingItem.RowNumber}. {updateResult.FirstError.Description}")) : 
            (new ImportBaseItem(existingPackaging.Value.Id, existingPackaging.Value.Code), null);  
    }

    private async Task<ErrorOr<Packaging>> CreatePackagingAsync(
        PackagingItem packagingItem, 
        CancellationToken cancellationToken)
    {
        var createCommand = new CreatePackagingCommand(
            new CreatePackagingRequest(
                packagingItem.PackagingTypeCode,
                packagingItem.DepositorCode,
                packagingItem.PackagingCirculationCode,
                packagingItem.CustomerNumber,
                packagingItem.Name,
                packagingItem.MustBeWashed,
                new DimensionsContract(
                    packagingItem.Depth,
                    packagingItem.Width,
                    packagingItem.Height),
                packagingItem.Weight,
                packagingItem.WashingMachineSpeeds
                    .Select(
                        x => new WashingMachineSpeedContract(x.WashingMachineCode, (WashingMachineSpeedLevelContract)x.Speed))
                    .ToArray()));

        return await sender.Send(createCommand, cancellationToken);
    }

    private async Task<ErrorOr<Updated>> UpdatePackagingAsync(
        PackagingItem packagingItem, 
        CancellationToken cancellationToken)
    {
        var updateCommand = new UpdatePackagingCommand(
            packagingItem.Code,
            new UpdatePackagingRequest(
                packagingItem.PackagingTypeCode,
                packagingItem.PackagingCirculationCode,
                packagingItem.Name,
                packagingItem.MustBeWashed,
                new DimensionsContract(
                    packagingItem.Depth,
                    packagingItem.Width,
                    packagingItem.Height),
                packagingItem.Weight,
                packagingItem.WashingMachineSpeeds
                    .Select(
                        x => new WashingMachineSpeedContract(x.WashingMachineCode, (WashingMachineSpeedLevelContract)x.Speed))
                    .ToArray()));

        return await sender.Send(updateCommand, cancellationToken);
    }
    
    private static void RemoveErrorPackagingCodesFromExistingItems(List<PackagingItem> items, List<ImportBaseError> errors)
    {
        var errorCodes = errors.Select(x => x.Code).ToHashSet();
        items.RemoveAll(x => errorCodes.Contains(x.Code));
    }
    
    private static (PackagingItem? Item, ImportBaseError? Error) GetItemResult(IXLRow row, List<WashingMachine> washingMachinesFromDb)
    {
        if (!row.Cell(PackagingHeader.Hmotnost).TryGetDecimalValue(out var weight))
        {
            var error = new ImportBaseError(GetPackagingCode(row), $"Číslo řádku: {row.RowNumber()}. Neplatná hodnota pro sloupec: {nameof(PackagingHeader.Hmotnost)}");
            return (null, error);
        }
            
        if (!row.Cell(PackagingHeader.Hloubka).TryGetDecimalValue(out var depth))
        {
            var error = new ImportBaseError(GetPackagingCode(row), $"Číslo řádku: {row.RowNumber()}. Neplatná hodnota pro sloupec: {nameof(PackagingHeader.Hloubka)}");
            return (null, error);
        }
        
        if (!row.Cell(PackagingHeader.Sirka).TryGetDecimalValue(out var width))
        {
            var error = new ImportBaseError(GetPackagingCode(row), $"Číslo řádku: {row.RowNumber()}. Neplatná hodnota pro sloupec: {nameof(PackagingHeader.Sirka)}");
            return (null, error);
        }
        
        if (!row.Cell(PackagingHeader.Vyska).TryGetDecimalValue(out var height))
        {
            var error = new ImportBaseError(GetPackagingCode(row), $"Číslo řádku: {row.RowNumber()}. Neplatná hodnota pro sloupec: {nameof(PackagingHeader.Vyska)}");
            return (null, error);
        }

        var washingMachineSpeedResult = TryGetWashingMachineSpeeds(
            row.Cell(PackagingHeader.RychlostiMycek), 
            out var washingMachineSpeeds);
        
        if (!washingMachineSpeedResult)
        {
            var error = new ImportBaseError(GetPackagingCode(row), $"Číslo řádku: {row.RowNumber()}. Neplatná hodnota pro sloupec: {nameof(PackagingHeader.RychlostiMycek)}");
            return (null, error);
        }
        
        var fullWashingMachineSpeeds = FillInWashingMachineSpeedsWithCodesFromDb(washingMachineSpeeds, washingMachinesFromDb);
        
        var item = new PackagingItem(
            code: row.Cell(PackagingHeader.KodUkladatele).GetValue<string>().Trim().ToUpperInvariant() 
                  + row.Cell(PackagingHeader.ZakaznickeCisloObalu).GetValue<string>().Trim().ToUpperInvariant(),
            packagingTypeCode: row.Cell(PackagingHeader.KodTypuObalu).GetValue<string>().Trim().ToUpperInvariant(),
            depositorCode: row.Cell(PackagingHeader.KodUkladatele).GetValue<string>().Trim().ToUpperInvariant(),
            packagingCirculationCode: row.Cell(PackagingHeader.KodObehovostiObalu).GetValue<string>().Trim().ToUpperInvariant(),
            customerNumber: row.Cell(PackagingHeader.ZakaznickeCisloObalu).GetValue<string>().Trim().ToUpperInvariant(),
            name: row.Cell(PackagingHeader.NazevObalu).GetValue<string>().Trim(),
            mustBeWashed: ConvertMustBeWashed(row.Cell(PackagingHeader.NutnoPrat).GetValue<string>()),
            depth: depth,
            width: width,
            height: height,
            weight: weight,
            fullWashingMachineSpeeds,
            rowNumber: row.RowNumber());
        
        return (item, null);
    }

    private static List<WashingMachineSpeed> FillInWashingMachineSpeedsWithCodesFromDb(
        List<WashingMachineSpeed> washingMachineSpeeds, List<WashingMachine> washingMachinesFromDb)
    {
        foreach (var washingMachineFromDb in washingMachinesFromDb)
        {
            var excelContainsCodeFromDb =
                washingMachineSpeeds.Any(x => x.WashingMachineCode.Equals(washingMachineFromDb.Code, StringComparison.CurrentCultureIgnoreCase));
            
            if (!excelContainsCodeFromDb)
                washingMachineSpeeds.Add(new WashingMachineSpeed(washingMachineFromDb.Code, WashingMachineSpeedLevel.NotSet));
        }
        
        return washingMachineSpeeds;
    }
    
    private static string GetPackagingCode(IXLRow row)
    {
        var depositorCode = row.Cell(PackagingHeader.KodUkladatele).GetTrimString().ToUpperInvariant();
        var customerNumber = row.Cell(PackagingHeader.ZakaznickeCisloObalu).GetTrimString().ToUpperInvariant();
        return depositorCode + customerNumber;
    }

    private static bool ConvertMustBeWashed(string value)
    {
        return value.Trim().ToLower() switch
        {
            "ne" => false,
            "false" => false,
            "0" => false,
            _ => true
        };
    }
    
    private static bool TryGetWashingMachineSpeeds(IXLCell cell, out List<WashingMachineSpeed> value)
    {
        value = [];
        
        var cellValue = cell.GetValue<string>().Trim();
        if (string.IsNullOrWhiteSpace(cellValue))
            return true; // this means we didn't set any speeds and they will be set to NotSet by default
        
        var washingMachinePairs = cellValue.Split(';', StringSplitOptions.RemoveEmptyEntries);;

        foreach (var washingMachinePair in washingMachinePairs)
        {
            var parts = washingMachinePair.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }
            
            var washingMachineCode = parts[0].Trim().ToUpperInvariant();
            var speedString = parts[1].Trim();
            
            if (!Enum.TryParse(speedString, out WashingMachineSpeedLevel speed) ||
                string.IsNullOrWhiteSpace(washingMachineCode))
            {
                return false;
            }
            
            value.Add(new WashingMachineSpeed(washingMachineCode, speed));
        }
        
        return true;
    }
}