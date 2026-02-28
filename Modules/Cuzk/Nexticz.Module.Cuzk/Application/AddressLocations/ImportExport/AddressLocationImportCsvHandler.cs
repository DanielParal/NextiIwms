using System.Globalization;
using System.Text;
using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.ImportsExports.Imports;
using Nexticz.Module.Cuzk.Application.AddressLocations.Commands.CreateUpdateBulkAddressLocations;
using Nexticz.Module.Cuzk.Application.ImportsExports.Imports.ImportHandlers;
using Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalityByCode;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.ImportExport;

internal class AddressLocationImportCsvHandler(
    ISender sender,
    ILogger<AddressLocationImportCsvHandler> logger) 
    : ImportCsvBaseTemplateHandler<ImportType>, ICuzkImportHandler<ImportType>
{
    private readonly Dictionary<string, Municipality?> _municipalityCache = new();
    protected override string[] ExpectedFileHeader { get; } = AddressLocationHeader.ExpectedFileHeader;
    protected override Encoding CsvEncoding { get; } = Encoding.GetEncoding(1250);
    protected override ImportType ImportType { get; } = ImportType.AddressLocationCsv;
    
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    protected override async Task<(List<object> Items, List<ImportBaseError> Errors)> GetItemsFromCsvAsync(IReadOnlyList<string?[]> rows, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            var items = new List<object>();
            var errors = new List<ImportBaseError>();

            if (rows.Count == 0 || rows[0].Length != ExpectedFileHeader.Length)
            {
                errors.Add(new ImportBaseError("unknown_format", $"Csv je buď prázdné nebo csv neobsahuje správný počet sloupců: {ExpectedFileHeader.Length}"));
                return (items, errors);
            }
        
            var rowNumber = 1;
        
        
            foreach (var row in rows.Skip(1)) // Skip the header row (row 1)
            {
                var addressLocationItem = await CreateAddressLocationItemAsync(row, rowNumber, cancellationToken);
            
                if (addressLocationItem.IsError)
                {
                    errors.Add(new ImportBaseError(addressLocationItem.FirstError.Code, addressLocationItem.FirstError.Description));
                    continue;
                }
            
                items.Add(addressLocationItem.Value);
            
                rowNumber++;
            }
            _municipalityCache.Clear();

            return (items, errors);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AddressLocationImportCsvHandler - error while getting items from csv.");
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    protected override async Task<(ImportBaseItem? SuccessItem, ImportBaseError? Error)[]> ProcessBulkItemsAsync(
        object[] items, 
        CancellationToken cancellationToken)
    {
        var itemsToReturn = new List<(ImportBaseItem? SuccessItem, ImportBaseError? Error)>();
        
        if (items.Length == 0)
            return itemsToReturn.ToArray();
        
        var bulkRequests = new List<CreateUpdateBulkAddressLocationsRequest>();
        foreach (var item in items)
        {
            if (item is not AddressLocationItem addressLocationItem)
            {
                itemsToReturn.Add(new ValueTuple<ImportBaseItem?, ImportBaseError?>(null, new ImportBaseError("", "Nesprávný typ položky při zpracování importu.")));
                continue;
            }
            
            var createAddressLocationRequest = new CreateUpdateBulkAddressLocationsRequest(
                addressLocationItem.AdmCode,
                addressLocationItem.MunicipalityCode,
                addressLocationItem.MunicipalityName,
                addressLocationItem.MunicipalityDistrictCode,
                addressLocationItem.MunicipalityDistrictName,
                addressLocationItem.MomcCode,
                addressLocationItem.MomcName,
                addressLocationItem.PragueDistrictCode,
                addressLocationItem.PragueDistrictName,
                addressLocationItem.StreetCode,
                addressLocationItem.StreetName,
                addressLocationItem.DistrictCode,
                addressLocationItem.DistrictName,
                addressLocationItem.CountryCode,
                addressLocationItem.CountryName,
                addressLocationItem.SoType,
                addressLocationItem.NumberDescriptive,
                addressLocationItem.NumberReference,
                addressLocationItem.NumberReferenceChar,
                addressLocationItem.ZipCode,
                addressLocationItem.KrovakX,
                addressLocationItem.KrovakY,
                addressLocationItem.Latitude,
                addressLocationItem.Longitude,
                null,
                addressLocationItem.ValidFrom);
            
            bulkRequests.Add(createAddressLocationRequest);
        }

        var bulkCreationResponse = await sender.Send(new CreateUpdateBulkAddressLocationsCommand(bulkRequests.ToArray()), cancellationToken);
        
        var succeededItems =
            bulkCreationResponse.SucceededItems
                .Select(x => 
                    new ValueTuple<ImportBaseItem?, ImportBaseError?>(new ImportBaseItem(x.Id!.Value, x.AdmCode), null))
                .ToArray();

        var failedItems = bulkCreationResponse.FailedItems
            .Select(x => 
                new ValueTuple<ImportBaseItem?, ImportBaseError?>(null, new ImportBaseError(x.AdmCode, x.ErrorMessage!)))
            .ToArray();
        
        itemsToReturn.AddRange(succeededItems);
        itemsToReturn.AddRange(failedItems);
        return itemsToReturn.ToArray();
    }

    private async Task<ErrorOr<AddressLocationItem>> CreateAddressLocationItemAsync(string?[] row, int rowNumber, CancellationToken cancellationToken)
    {
        var admCode = row[0]?.Trim();
        var municipalityCode = row[1]?.Trim();
        
        if (string.IsNullOrWhiteSpace(admCode)
            || string.IsNullOrWhiteSpace(municipalityCode))
        {
            return Error.Failure(admCode ?? "", $"Číslo řádku: {rowNumber}. Nulová hodnota.");
        }
        
        var municipalityName = row[2]?.Trim();
        var momcCode = row[3]?.Trim();
        var momcName = row[4]?.Trim();
        var pragueDistrictCode = row[5]?.Trim();
        var pragueDistrictName = row[6]?.Trim();
        var municipalityDistrictCode = row[7]?.Trim();
        var municipalityDistrictName = row[8]?.Trim();

        var municipality = await GetMunicipalityAsync(municipalityCode, cancellationToken);
        
        var districtCode = municipality?.DistrictCode;
        var districtName = municipality?.DistrictName;
        var countryCode = municipality?.VuscCode;
        var countryName = municipality?.VuscName;
        
        var streetCode = row[9]?.Trim();
        var streetName = row[10]?.Trim();
        var soType = row[11]?.Trim();
        var numberDescriptive = row[12]?.Trim();
        var numberReference = row[13]?.Trim();
        var numberReferenceChar = row[14]?.Trim();
        var zipCode = row[15]?.Trim();
        var krovakY = row[16]?.Trim();
        var krovakX = row[17]?.Trim();
        
        var latitude = string.Empty;
        var longitude = string.Empty;
        var culture = CultureInfo.InvariantCulture;
        if (double.TryParse(krovakX, NumberStyles.Any, culture, out var xCoordinate) &&
            double.TryParse(krovakY, NumberStyles.Any, culture, out var yCoordinate))
        {
            var coordinates = AddressLocation.ConvertKrovakToLatLon(xCoordinate, yCoordinate);
            latitude = coordinates.Latitude.ToString(culture);
            longitude = coordinates.Longitude.ToString(culture);
        }
        
        var validFrom = row[18]?.Trim();
        DateTime? validFromDateTime = DateTime.TryParse(validFrom, out var dateTime) ? dateTime : null;

        return new AddressLocationItem(
            admCode!,
            municipalityCode!,
            municipalityName,
            municipalityDistrictCode,
            municipalityDistrictName,
            momcCode,
            momcName,
            pragueDistrictCode,
            pragueDistrictName,
            streetCode,
            streetName,
            districtCode,
            districtName,
            countryCode,
            countryName,
            soType,
            numberDescriptive,
            numberReference,
            numberReferenceChar,
            zipCode,
            krovakX,
            krovakY,
            latitude,
            longitude,
            validFromDateTime,
            rowNumber);
    }

    private async Task<Municipality?> GetMunicipalityAsync(string municipalityCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(municipalityCode))
            return null;
        
        if (_municipalityCache.TryGetValue(municipalityCode, out var municipalityFromCache))
            return municipalityFromCache;
        
        var municipality = await sender.Send(new GetMunicipalityByCodeQuery(municipalityCode), cancellationToken);
        var municipalityValue = municipality.IsError ? null : municipality.Value;
        
        _municipalityCache.Add(municipalityCode, municipalityValue);

        return municipalityValue;
    }
}