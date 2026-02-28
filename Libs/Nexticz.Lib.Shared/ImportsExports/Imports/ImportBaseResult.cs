using ErrorOr;

namespace Nexticz.Lib.Shared.ImportsExports.Imports;

public class ImportBaseResult(List<ImportBaseItem> items, List<ImportBaseError> errors)
{
    public List<ImportBaseItem> SuccessfullyImportedItems { get; private set; } = items;
    public List<ImportBaseError> Errors { get; private set; } = errors;
    public ImportBaseStatus Status 
    {
        get
        {
            if (SuccessfullyImportedItems.Count == 0)
                return ImportBaseStatus.Failure;
        
            return Errors.Count != 0 ? ImportBaseStatus.PartialSuccessWithErrors : ImportBaseStatus.Success;
        }
    }

    public static ImportBaseResult CreateWithError(Error error)
    {
        return new ImportBaseResult([], [new ImportBaseError(error)]);
    }
    
    public static ImportBaseResult CreateWithErrors(List<Error> errors)
    {
        return new ImportBaseResult([], errors.Select(x => new ImportBaseError(x)).ToList());
    }
}