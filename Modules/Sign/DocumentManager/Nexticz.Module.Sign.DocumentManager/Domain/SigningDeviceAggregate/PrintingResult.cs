using ErrorOr;

namespace Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

public class PrintingResult
{
    public PrintingResult(bool isSuccess, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }
    
    public PrintingResult(ErrorOr<Success> printingResult)
    {
        if (printingResult.IsError)
        {
            IsSuccess = false;
            ErrorMessage = printingResult.FirstError.Description;
            return;
        }
        
        IsSuccess = true;
        ErrorMessage = null;
    }

    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
}