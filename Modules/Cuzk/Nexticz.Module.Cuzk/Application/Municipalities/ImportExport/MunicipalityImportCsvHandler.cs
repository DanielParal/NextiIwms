using System.Text;
using MediatR;
using Nexticz.Lib.Shared.ImportsExports.Imports;
using Nexticz.Module.Cuzk.Application.ImportsExports.Imports.ImportHandlers;
using Nexticz.Module.Cuzk.Application.Municipalities.Commands.CreateUpdateBulkMunicipalities;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.Municipalities.ImportExport;

internal class MunicipalityImportCsvHandler(
    ISender sender) 
    : ImportCsvBaseTemplateHandler<ImportType>, ICuzkImportHandler<ImportType>
{
    protected override string[] ExpectedFileHeader { get; } = MunicipalityHeader.ExpectedFileHeader;
    protected override Encoding CsvEncoding { get; } = Encoding.GetEncoding(1250);
    protected override ImportType ImportType { get; } = ImportType.MunicipalityCsv;

    protected override Task<(List<object> Items, List<ImportBaseError> Errors)> GetItemsFromCsvAsync(IReadOnlyList<string?[]> rows, CancellationToken cancellationToken)
    {
        var items = new List<object>();
        var errors = new List<ImportBaseError>();

        if (rows.Count == 0 || rows[0].Length != ExpectedFileHeader.Length)
        {
            errors.Add(new ImportBaseError("unknown_format", $"Csv je buď prázdné nebo csv neobsahuje správný počet sloupců: {ExpectedFileHeader.Length}"));
            return Task.FromResult((items, errors));
        }
        
        var rowNumber = 1;
        
        foreach (var row in rows.Skip(1)) // Skip the header row (row 1)
        {
            var code = row[0]?.Trim();
            var name = row[1]?.Trim();
            var status = row[2]?.Trim();
            var pouCode = row[3]?.Trim();
            var pouName = row[4]?.Trim();
            var orpCode = row[5]?.Trim();
            var orpName = row[6]?.Trim();
            var districtCode = row[7]?.Trim();
            var districtName = row[8]?.Trim();
            var vuscCode = row[9]?.Trim();
            var vuscName = row[10]?.Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                errors.Add(new ImportBaseError(code ?? "", $"Číslo řádku: {rowNumber}. Nulová hodnota."));
                continue;
            }
            
            items.Add(new MunicipalityItem(
                code!, name, status, pouCode, pouName, orpCode, orpName, districtCode, 
                districtName, vuscCode, vuscName, rowNumber));
            
            rowNumber++;
        }
        
        return Task.FromResult((items, errors));
    }

    protected override async Task<(ImportBaseItem? SuccessItem, ImportBaseError? Error)[]> ProcessBulkItemsAsync(
        object[] items,
        CancellationToken cancellationToken)
    {
        var itemsToReturn = new List<(ImportBaseItem? SuccessItem, ImportBaseError? Error)>();
        
        if (items.Length == 0)
            return itemsToReturn.ToArray();
        
        var bulkRequests = new List<CreateUpdateBulkMunicipalitiesRequest>();
        
        foreach (var item in items)
        {
            if (item is not MunicipalityItem municipalityItem)
            {
                itemsToReturn.Add(new ValueTuple<ImportBaseItem?, ImportBaseError?>(null, new ImportBaseError("", "Nesprávný typ položky při zpracování importu.")));
                continue;
            }
            
            var createMunicipalityRequest = new CreateUpdateBulkMunicipalitiesRequest(
                municipalityItem.Code,
                municipalityItem.Name,
                municipalityItem.Status,
                municipalityItem.PouCode,
                municipalityItem.PouName,
                municipalityItem.OrpCode,
                municipalityItem.OrpName,
                municipalityItem.DistrictCode,
                municipalityItem.DistrictName,
                municipalityItem.VuscCode,
                municipalityItem.VuscName,
                true);
            
            bulkRequests.Add(createMunicipalityRequest);
        }
        
        var bulkCreationResponse = await sender.Send(new CreateUpdateBulkMunicipalitiesCommand(bulkRequests.ToArray()), cancellationToken);
        
        var succeededItems =
            bulkCreationResponse.SucceededItems
                .Select(x => 
                    new ValueTuple<ImportBaseItem?, ImportBaseError?>(new ImportBaseItem(x.Id!.Value, x.Code), null))
                .ToArray();

        var failedItems = bulkCreationResponse.FailedItems
            .Select(x => 
                new ValueTuple<ImportBaseItem?, ImportBaseError?>(null, new ImportBaseError(x.Code, x.ErrorMessage!)))
            .ToArray();
        
        itemsToReturn.AddRange(succeededItems);
        itemsToReturn.AddRange(failedItems);
        return itemsToReturn.ToArray();
    }
}