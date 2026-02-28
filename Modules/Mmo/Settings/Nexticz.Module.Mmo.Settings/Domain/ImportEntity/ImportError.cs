using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Domain.ImportEntity;

public class ImportError
{
    public string Code { get; private set; }
    public string Message { get; private set; }

    public ImportError()
    {
    }
    
    public ImportError(string code, string message)
    {
        Code = code;
        Message = message;
    }
    
    public ImportError(Error error)
    {
        Code = error.Code;
        Message = error.Description;
    }
};