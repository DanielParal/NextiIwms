using ClosedXML.Excel;
using MediatR;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.ImportsExports.Imports;
using Nexticz.Module.Sign.Settings.Application.Depositors.Commands.CreateDepositor;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Imports.ImportHandlers;
using Nexticz.Module.Sign.Settings.Domain.ImportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.ImportExport;

internal class DepositorImportXlsxHandler(
    ISender sender) 
    : ImportXlsxBaseTemplateHandler<ImportType>, ISettingsImportHandler<ImportType>
{
    protected override string[] ExpectedFileHeader { get; } = DepositorHeader.ExpectedFileHeader;
    protected override ImportType ImportType { get; } = ImportType.DepositorXlsx;
    
    protected override Task<(List<object> Items, List<ImportBaseError> Errors)> GetItemsFromExcelAsync(XLWorkbook workbook, CancellationToken cancellationToken)
    {
        var items = new List<object>();
        var errors = new List<ImportBaseError>();
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RowsUsed();

        foreach (var row in rows.Skip(1)) // Skip the header row (row 1)
        {
            var code = row.Cell(DepositorHeader.Kod).GetTrimString().ToUpperInvariant();
            var name = row.Cell(DepositorHeader.Jmeno).GetTrimString();
            var depositorGroupCode = row.Cell(DepositorHeader.KodSkupinyUkladatelu).GetTrimString();
            var deliveryTemplateCode = row.Cell(DepositorHeader.KodSablonyDodacihoListu).GetTrimString();
            var loadingTemplateCode = row.Cell(DepositorHeader.KodSablonyNakladnihoListu).GetTrimString();

            if (string.IsNullOrWhiteSpace(code) 
                || string.IsNullOrWhiteSpace(name)
                || string.IsNullOrWhiteSpace(depositorGroupCode)
                || string.IsNullOrWhiteSpace(deliveryTemplateCode)
                || string.IsNullOrWhiteSpace(loadingTemplateCode))
            {
                errors.Add(new ImportBaseError(code, $"Číslo řádku: {row.RowNumber()}. Nulová hodnota."));
                continue;
            }
            
            items.Add(new DepositorItem(code, name, depositorGroupCode, deliveryTemplateCode, loadingTemplateCode, row.RowNumber()));
        }
        
        return Task.FromResult((items, errors));
    }
    
    protected override async Task<(ImportBaseItem? SuccessItem, ImportBaseError? Error)> ProcessItemAsync(
        object item, 
        CancellationToken cancellationToken)
    {
        if (item is not DepositorItem depositorItem)
            return (null, new ImportBaseError("", "Nesprávný typ položky při zpracování importu."));
        
        var createResult = await sender.Send(new CreateDepositorCommand(
            depositorItem.Code, depositorItem.Name, depositorItem.DepositorGroupCode, depositorItem.DeliveryTemplateCode, depositorItem.LoadingTemplateCode), 
            cancellationToken);
        
        return createResult.IsError ? 
            (null, new ImportBaseError(depositorItem.Code, $"Číslo řádku: {depositorItem.RowNumber}. {createResult.FirstError.Description}")) : 
            (new ImportBaseItem(createResult.Value.Id, createResult.Value.Code), null);
    }
}