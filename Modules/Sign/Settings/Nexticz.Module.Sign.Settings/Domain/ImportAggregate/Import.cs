using Nexticz.Module.Sign.SharedKernel.DomainCore;
using Nexticz.Lib.Shared.ImportsExports.Imports;
using Nexticz.Module.Sign.Settings.Domain.ImportAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Domain.ImportAggregate;

public class Import : AggregateRoot
{
    public string UserName { get; private set; }
    public ImportType Type { get; private set; }
    public ImportStatus Status { get; private set; }
    public string[] ImportedCodes { get; private set; }
    public ImportError[] Errors { get; private set; }
    public DateTimeOffset DateCreated { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Import() {}
    
    private Import(
        string userName,
        ImportType type,
        ImportStatus status,
        string[] importedCodes,
        ImportError[] errors,
        DateTimeOffset dateCreated,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        UserName = userName;
        Type = type;
        Status = status;
        ImportedCodes = importedCodes;
        Errors = errors;
        DateCreated = dateCreated;
    }

    public static Import CreateFrom(string userName, ImportType type, DateTimeOffset dateCreated, ImportBaseResult importResult)
    {
        var status = (ImportStatus)importResult.Status;
        var importedCodes = importResult.SuccessfullyImportedItems.Select(x => x.Code).Distinct().ToArray();
        var errors = importResult.Errors.Select(x => new ImportError(x.Code, x.Message)).ToArray();

        return new Import(userName, type, status, importedCodes, errors, dateCreated);
    }
    
    public void Apply(ImportCreatedEvent @event)
    {
        Id = @event.Id;
        UserName = @event.UserName;
        Type = @event.Type;
        Status = @event.Status;
        ImportedCodes = @event.ImportedCodes;
        Errors = @event.Errors;
        DateCreated = @event.DateCreated;
    }
}