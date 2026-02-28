using Nexticz.Lib.Shared.ImportsExports.Imports;
using Nexticz.Module.Cuzk.Domain.ImportAggregate.Events;

namespace Nexticz.Module.Cuzk.Domain.ImportAggregate;

public class Import : AggregateRoot
{
    public string UserName { get; private set; }
    public string FileName { get; private set; }
    public string? CsvDelimiter { get; private set; }
    public ImportType Type { get; private set; }
    public ImportStatus Status { get; private set; }
    public string[] ImportedCodes { get; private set; }
    public ImportError[] Errors { get; private set; }
    public DateTimeOffset DateRequested { get; private set; }
    public DateTimeOffset DateCreated { get; private set; }
    public DateTimeOffset? DateImported { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Import() {}
    
    private Import(
        string userName,
        ImportType type, 
        string fileName,
        string? csvDelimiter,
        DateTimeOffset dateRequested,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        UserName = userName;
        Type = type;
        Status = ImportStatus.Requested;
        FileName = fileName;
        CsvDelimiter = csvDelimiter;
        DateRequested = dateRequested;
        DateCreated = dateRequested;
        DateImported = null;
        ImportedCodes = [];
        Errors = [];
    }

    public static Import CreateFrom(Guid id, string userName, ImportType type, string fileName, string? csvDelimiter, DateTimeOffset dateRequested)
    {
        return new Import(userName, type, fileName, csvDelimiter, dateRequested, id: id);
    }

    public void ProcessImport(DateTimeOffset dateImported, ImportBaseResult importResult)
    {
        Status = (ImportStatus)importResult.Status;
        ImportedCodes = importResult.SuccessfullyImportedItems.Select(x => x.Code).Distinct().ToArray();
        Errors = importResult.Errors.Select(x => new ImportError(x.Code, x.Message)).ToArray();
        DateImported = dateImported;
    }
    
    public void Apply(ImportRequestedEvent @event)
    {
        Id = @event.Id;
        Type = @event.Type;
        UserName = @event.UserName;
        Status = ImportStatus.Requested;
        FileName = @event.FileName;
        CsvDelimiter = @event.CsvDelimiter;
        DateRequested = @event.DateRequested;
        DateCreated = @event.DateRequested;
        DateImported = null;
        ImportedCodes = [];
        Errors = [];
    }
    
    public void Apply(RequestedImportProcessedEvent @event)
    {
        Id = @event.Id;
        Status = @event.Status;
        ImportedCodes = @event.ImportedCodes;
        Errors = @event.Errors;
        DateImported = @event.DateImported;
    }
}