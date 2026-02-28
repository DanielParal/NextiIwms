using ClosedXML.Excel;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.ImportsExports;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.ImportsExports.Imports;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Imports.ImportHandlers;
using Nexticz.Module.Sign.Settings.Application.Partners.Commands.CreatePartner;
using Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartnerByCode;
using Nexticz.Module.Sign.Settings.Domain.ImportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Partners.ImportExport;

internal class PartnerImportXlsxHandler(
    ISender sender) 
    : ImportXlsxBaseTemplateHandler<ImportType>, ISettingsImportHandler<ImportType>
{
    protected override string[] ExpectedFileHeader { get; } = PartnerHeader.ExpectedFileHeader;
    protected override ImportType ImportType { get; } = ImportType.PartnerXlsx;
    
    protected override Task<(List<object> Items, List<ImportBaseError> Errors)> GetItemsFromExcelAsync(XLWorkbook workbook, CancellationToken cancellationToken)
    {
        var items = new List<object>();
        var errors = new List<ImportBaseError>();
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RowsUsed();

        foreach (var row in rows.Skip(1)) // Skip the header row (row 1)
        {
            var code = row.Cell(PartnerHeader.Kod).GetTrimString().ToUpperInvariant();
            var name = row.Cell(PartnerHeader.Jmeno).GetTrimString();

            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
            {
                errors.Add(new ImportBaseError(code, $"Číslo řádku: {row.RowNumber()}. Nulová hodnota pro kód nebo jméno."));
                continue;
            }
            
            items.Add(new PartnerItem(code, name, row.RowNumber()));
        }
        
        return Task.FromResult((items, errors));
    }
    
    protected override async Task<(ImportBaseItem? SuccessItem, ImportBaseError? Error)> ProcessItemAsync(
        object item, 
        CancellationToken cancellationToken)
    {
        if (item is not PartnerItem partnerItem)
            return (null, new ImportBaseError("", "Nesprávný typ položky při zpracování importu."));
        
        var existingPartner = await sender.Send(new GetPartnerByCodeQuery(partnerItem.Code), cancellationToken);

        if (existingPartner.HasValue())
            return (null, new ImportBaseError(partnerItem.Code, $"Číslo řádku: {partnerItem.RowNumber}. Partner s tímto kódem již existuje."));
        
        var createResult = await sender.Send(new CreatePartnerCommand(partnerItem.Code, partnerItem.Name), cancellationToken);
        return createResult.IsError ? 
            (null, new ImportBaseError(partnerItem.Code, $"Číslo řádku: {partnerItem.RowNumber}. {createResult.FirstError.Description}")) : 
            (new ImportBaseItem(createResult.Value.Id, createResult.Value.Code), null);
    }
}