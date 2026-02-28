using ClosedXML.Excel;
using MediatR;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.ImportsExports.Imports;
using Nexticz.Module.Sign.Settings.Application.DepositorGroups.Commands.CreateDepositorGroup;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Imports.ImportHandlers;
using Nexticz.Module.Sign.Settings.Domain.ImportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.ImportExport;

internal class DepositorGroupImportXlsxHandler(
    ISender sender) 
    : ImportXlsxBaseTemplateHandler<ImportType>, ISettingsImportHandler<ImportType>
{
    protected override string[] ExpectedFileHeader { get; } = DepositorGroupHeader.ExpectedFileHeader;
    protected override ImportType ImportType { get; } = ImportType.DepositorGroupXlsx;
    
    protected override Task<(List<object> Items, List<ImportBaseError> Errors)> GetItemsFromExcelAsync(XLWorkbook workbook, CancellationToken cancellationToken)
    {
        var items = new List<object>();
        var errors = new List<ImportBaseError>();
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RowsUsed();

        foreach (var row in rows.Skip(1)) // Skip the header row (row 1)
        {
            var code = row.Cell(DepositorGroupHeader.Kod).GetTrimString().ToUpperInvariant();
            var name = row.Cell(DepositorGroupHeader.Jmeno).GetTrimString();

            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
            {
                errors.Add(new ImportBaseError(code, $"Číslo řádku: {row.RowNumber()}. Nulová hodnota pro kód nebo jméno."));
                continue;
            }
            
            items.Add(new DepositorGroupItem(code, name, row.RowNumber()));
        }
        
        return Task.FromResult((items, errors));
    }
    
    protected override async Task<(ImportBaseItem? SuccessItem, ImportBaseError? Error)> ProcessItemAsync(
        object item, 
        CancellationToken cancellationToken)
    {
        if (item is not DepositorGroupItem depositorGroupItem)
            return (null, new ImportBaseError("", "Nesprávný typ položky při zpracování importu."));
        
        var createResult = await sender.Send(new CreateDepositorGroupCommand(depositorGroupItem.Code, depositorGroupItem.Name), cancellationToken);
        return createResult.IsError ? 
            (null, new ImportBaseError(depositorGroupItem.Code, $"Číslo řádku: {depositorGroupItem.RowNumber}. {createResult.FirstError.Description}")) : 
            (new ImportBaseItem(createResult.Value.Id, createResult.Value.Code), null);
    }
}