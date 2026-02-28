using ClosedXML.Excel;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.ImportsExports;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.ImportsExports.Imports;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Imports.ImportHandlers;
using Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartnerByCode;
using Nexticz.Module.Sign.Settings.Application.Receivers.Commands.CreateReceiver;
using Nexticz.Module.Sign.Settings.Application.Receivers.Queries.GetReceiverByCodeAndPartnerCode;
using Nexticz.Module.Sign.Settings.Domain.ImportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.ImportExport;

internal class ReceiverImportXlsxHandler(
    ISender sender) 
    : ImportXlsxBaseTemplateHandler<ImportType>, ISettingsImportHandler<ImportType>
{
    protected override string[] ExpectedFileHeader { get; } = ReceiverHeader.ExpectedFileHeader;
    protected override ImportType ImportType { get; } = ImportType.ReceiverXlsx;
    
    protected override Task<(List<object> Items, List<ImportBaseError> Errors)> GetItemsFromExcelAsync(XLWorkbook workbook, CancellationToken cancellationToken)
    {
        var items = new List<object>();
        var errors = new List<ImportBaseError>();
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RowsUsed();

        foreach (var row in rows.Skip(1)) // Skip the header row (row 1)
        {
            var code = row.Cell(ReceiverHeader.Kod).GetTrimString().ToUpperInvariant();
            var name = row.Cell(ReceiverHeader.Jmeno).GetTrimString();
            var partnerCode = row.Cell(ReceiverHeader.KodPartnera).GetTrimString();

            if (string.IsNullOrWhiteSpace(code) 
                || string.IsNullOrWhiteSpace(name)
                || string.IsNullOrWhiteSpace(partnerCode))
            {
                errors.Add(new ImportBaseError($"{code}-{partnerCode}", $"Číslo řádku: {row.RowNumber()}. Nulová hodnota pro kódy nebo jméno."));
                continue;
            }
            
            items.Add(new ReceiverItem(code, name, partnerCode, row.RowNumber()));
        }
        
        return Task.FromResult((items, errors));
    }
    
    protected override async Task<(ImportBaseItem? SuccessItem, ImportBaseError? Error)> ProcessItemAsync(
        object item, 
        CancellationToken cancellationToken)
    {
        if (item is not ReceiverItem receiverItem)
            return (null, new ImportBaseError("", "Nesprávný typ položky při zpracování importu."));
        
        var existingPartner = await sender.Send(new GetPartnerByCodeQuery(receiverItem.PartnerCode), cancellationToken);

        if (!existingPartner.HasValue())
            return (null, new ImportBaseError($"{receiverItem.Code}-{receiverItem.PartnerCode}", 
                $"Číslo řádku: {receiverItem.RowNumber}. Partner s tímto kódem neexistuje."));

        var existingCombination = await sender.Send(new GetReceiverByCodeAndPartnerCodeQuery(receiverItem.Code, receiverItem.PartnerCode), cancellationToken);;
        
        if (existingCombination.HasValue())
            return (null, new ImportBaseError($"{receiverItem.Code}-{receiverItem.PartnerCode}", 
                $"Číslo řádku: {receiverItem.RowNumber}. Kombinace partnera a příjemce již existuje."));
        
        var createResult = await sender.Send(new CreateReceiverCommand(receiverItem.Code, receiverItem.PartnerCode, receiverItem.Name), cancellationToken);
        return createResult.IsError ? 
            (null, new ImportBaseError($"{receiverItem.Code}-{receiverItem.PartnerCode}", 
                $"Číslo řádku: {receiverItem.RowNumber}. {createResult.FirstError.Description}")) : 
            (new ImportBaseItem(createResult.Value.Id, $"{createResult.Value.Code}-{createResult.Value.PartnerCode}"), null);
    }
}