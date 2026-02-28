using ClosedXML.Excel;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Nexticz.Lib.Shared.ImportsExports.Imports;

public abstract class ImportXlsxBaseTemplateHandler<TImportType>
{
    protected abstract TImportType ImportType { get; }
    protected abstract string[] ExpectedFileHeader { get; }
    
    public bool CanHandle(TImportType importType)
    {
        return EqualityComparer<TImportType>.Default.Equals(ImportType, importType);
    }
    
    public async Task<ImportBaseResult> HandleAsync(object data, CancellationToken cancellationToken)
    {
        if (data is not IFormFile file) 
            return ImportBaseResult.CreateWithError(ImportErrors.ValidationFileIsNotProvided);
        
        if (!IsExcelFile(file))
            return ImportBaseResult.CreateWithError(ImportErrors.ValidationFileIsNotExcel);
        
        await using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);
        
        var isValidFileStructure = ValidateFileStructure(workbook);

        if (isValidFileStructure.IsError)
            return ImportBaseResult.CreateWithErrors(isValidFileStructure.Errors);
        
        return await ImportDataAsync(workbook, cancellationToken);
    }
    
    private async Task<ImportBaseResult> ImportDataAsync(XLWorkbook workbook, CancellationToken cancellationToken)
    {
        var (items, errors) = await GetItemsFromExcelAsync(workbook, cancellationToken);
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
    
    protected abstract Task<(List<object> Items, List<ImportBaseError> Errors)> GetItemsFromExcelAsync(XLWorkbook workbook, CancellationToken cancellationToken);
    protected abstract Task<(ImportBaseItem? SuccessItem, ImportBaseError? Error)> ProcessItemAsync(object item, CancellationToken cancellationToken);
    
    private static bool IsExcelFile(IFormFile file)
    {
        var validExcelContentTypes = new HashSet<string>
        {
            "application/vnd.ms-excel", // .xls
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" // .xlsx
        };
        
        return validExcelContentTypes.Contains(file.ContentType);
    }
    
    private ErrorOr<Success> ValidateFileStructure(XLWorkbook workbook)
    {
        var worksheet = workbook.Worksheet(1);
        if (!IsHeaderValid(worksheet))
            return ImportErrors.ValidationFileHasWrongFormat(string.Join(", ", ExpectedFileHeader));
        
        if (!HasExcelData(worksheet))
            return ImportErrors.ValidationFileHasNoData;
        
        return Result.Success;
    }

    private static bool HasExcelData(IXLWorksheet worksheet)
    {
        return worksheet.Worksheet.RowsUsed().Count() > 1;
    }
    
    private bool IsHeaderValid(IXLWorksheet worksheet)
    {
        var row = worksheet.Row(1);
        
        for (var i = 0; i < ExpectedFileHeader.Length; i++)
        {
            var cellValue = row.Cell(i + 1).GetValue<string>().Trim();
            if (!cellValue.Equals(ExpectedFileHeader[i], StringComparison.OrdinalIgnoreCase)) 
                return false;
        }
        
        return true;
    }
}