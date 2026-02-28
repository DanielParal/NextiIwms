using System.Globalization;
using System.Text;
using CsvHelper;
using ErrorOr;
using Nexticz.Lib.Shared.ImportsExports.Imports.ImportFiles;

namespace Nexticz.Lib.Shared.ImportsExports.Imports;

public abstract class ImportCsvBaseTemplateHandler<TImportType>
{
    protected abstract TImportType ImportType { get; }
    protected abstract string[] ExpectedFileHeader { get; }
    protected abstract Encoding CsvEncoding { get; }

    public bool CanHandle(TImportType importType)
    {
        return EqualityComparer<TImportType>.Default.Equals(ImportType, importType);
    }

    public async Task<ImportBaseResult> HandleAsync(IImportFile file, CancellationToken cancellationToken, string? csvDelimiter = null, bool bulkImport = false)
    {
        if (!IsCsvFile(file))
            return ImportBaseResult.CreateWithError(ImportErrors.ValidationFileIsNotCsv);

        await using var stream = file.Stream;
        var rows = ParseCsv(stream, csvDelimiter);

        var isValidFileStructure = ValidateFileStructure(rows);
        if (isValidFileStructure.IsError)
            return ImportBaseResult.CreateWithErrors(isValidFileStructure.Errors);

        await file.DisposeAsync();
        return bulkImport ? await ImportBulkDataAsync(rows, cancellationToken) : await ImportDataAsync(rows, cancellationToken);
    }

    private async Task<ImportBaseResult> ImportDataAsync(IReadOnlyList<string?[]> rows, CancellationToken cancellationToken)
    {
        var (items, errors) = await GetItemsFromCsvAsync(rows, cancellationToken);
        var successfullyImportedItems = new List<ImportBaseItem>();

        foreach (var item in items)
        {
            var result = await ProcessItemAsync(item, cancellationToken);
            if (result.Error != null)
            {
                errors.Add(result.Error);
                continue;
            }

            successfullyImportedItems.Add(result.SuccessItem!);
        }

        return new ImportBaseResult(successfullyImportedItems, errors);
    }
    
    private async Task<ImportBaseResult> ImportBulkDataAsync(IReadOnlyList<string?[]> rows, CancellationToken cancellationToken)
    {
        var (items, errors) = await GetItemsFromCsvAsync(rows, cancellationToken);
        var successfullyImportedItems = new List<ImportBaseItem>();

        var result = await ProcessBulkItemsAsync(items.ToArray(), cancellationToken);

        successfullyImportedItems.AddRange(result.Where(r => r.SuccessItem != null).Select(r => r.SuccessItem!));
        errors.AddRange(result.Where(r => r.Error != null).Select(r => r.Error!));
        
        return new ImportBaseResult(successfullyImportedItems, errors);
    }
    
    protected abstract Task<(List<object> Items, List<ImportBaseError> Errors)> GetItemsFromCsvAsync(IReadOnlyList<string?[]> rows, CancellationToken cancellationToken);

    protected virtual Task<(ImportBaseItem? SuccessItem, ImportBaseError? Error)> ProcessItemAsync(object item,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Implement this method in the derived class in case it is needed.");
    }

    protected virtual Task<(ImportBaseItem? SuccessItem, ImportBaseError? Error)[]> ProcessBulkItemsAsync(object[] item,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Implement this method in the derived class in case it is needed.");
    }

    private static bool IsCsvFile(IImportFile file)
    {
        var validCsvContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "text/csv",
            "application/csv",
            "application/vnd.ms-excel"
        };

        if (file.ContentType != null && validCsvContentTypes.Contains(file.ContentType))
            return true;

        return Path.GetExtension(file.FileName)
            .Equals(".csv", StringComparison.OrdinalIgnoreCase);
    }

    private ErrorOr<Success> ValidateFileStructure(IReadOnlyList<string?[]> rows)
    {
        if (rows.Count == 0)
            return ImportErrors.ValidationFileHasNoData;

        if (!IsHeaderValid(rows[0]))
            return ImportErrors.ValidationFileHasWrongFormat(string.Join(", ", ExpectedFileHeader));

        if (!HasCsvData(rows))
            return ImportErrors.ValidationFileHasNoData;

        return Result.Success;
    }

    private static bool HasCsvData(IReadOnlyList<string?[]> rows)
    {
        return rows.Count > 1; // header + at least one data row
    }

    private bool IsHeaderValid(string?[] headerRow)
    {
        if (headerRow.Length < ExpectedFileHeader.Length)
            return false;

        for (var i = 0; i < ExpectedFileHeader.Length; i++)
        {
            var cellValue = (headerRow[i] ?? string.Empty).Trim();
            if (!cellValue.Equals(ExpectedFileHeader[i], StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    
    private List<string?[]> ParseCsv(Stream stream, string? csvDelimiter)
    {
        using var reader = new StreamReader(stream, CsvEncoding, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        using var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = string.IsNullOrWhiteSpace(csvDelimiter) ? ";" : csvDelimiter,
            HasHeaderRecord = true,
            MissingFieldFound = null
        });

        var records = new List<string?[]>();
        while (csv.Read())
        {
            var record = new string?[csv.Parser.Count];
            for (var i = 0; i < csv.Parser.Count; i++)
            {
                record[i] = csv.GetField(i);
            }
            records.Add(record);
        }
            
        return records;
    }
}