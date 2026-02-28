using ClosedXML.Excel;
using Nexticz.Lib.Shared.FileHandling.Models;

namespace Nexticz.Lib.Shared.ImportsExports.Exports;

public abstract class ExportXlsxBaseTemplateHandler<TEntity, TExportType>
{
    protected abstract TExportType ExportType { get; }
    protected abstract string[] Headers { get; }
    protected abstract string WorksheetName { get; }
    protected abstract string FileNamePrefix { get; }
    
    public async Task<FileResult> HandleAsync(CancellationToken cancellationToken)
    {
        var data = await FetchDataAsync(cancellationToken);
        
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(WorksheetName);
        
        AddHeaders(worksheet);
        
        PopulateData(worksheet, data);
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        
        return new FileResult(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"{FileNamePrefix}_{DateTimeOffset.Now:yyyyMMddHHmmss}.xlsx");
    }

    public bool CanHandle(TExportType exportType)
    {
        return EqualityComparer<TExportType>.Default.Equals(ExportType, exportType);
    }
    
    protected virtual void AddHeaders(IXLWorksheet worksheet)
    {
        for (var i = 0; i < Headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = Headers[i];
            worksheet.Cell(1, i + 1).Style.Font.SetBold();
        }
    }

    protected abstract Task<List<TEntity>> FetchDataAsync(CancellationToken cancellationToken);
    protected abstract void PopulateData(IXLWorksheet worksheet, List<TEntity> entities);
}