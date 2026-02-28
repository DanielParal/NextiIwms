using ErrorOr;

namespace Nexticz.Lib.Shared.ImportsExports.Imports;

public class ImportBaseError
{
    public string Code { get; private set; }
    public string Message { get; private set; }

    public ImportBaseError()
    {
    }
    
    public ImportBaseError(string code, string message)
    {
        Code = code;
        Message = message;
    }
    
    public ImportBaseError(Error error)
    {
        Code = error.Code;
        Message = error.Description;
    }
}