using ErrorOr;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Logging;


namespace Nexticz.Lib.Shared.Errors;

public static class ErrorOrExtension
{
    public static ApiErrorResponse MapToErrorResponse(this Error error)
    {
        return new ApiErrorResponse
        {
            CorrelationId = CorrelationIdProvider.Instance.GetInternalId(),
            Errors = [new ApiError { Message = error.Description, Slug = error.Code }]
        };
    }
    
    public static ApiErrorResponse MapToErrorResponse(this List<Error> errors)
    {
        return new ApiErrorResponse
        {
            CorrelationId = CorrelationIdProvider.Instance.GetInternalId(),
            Errors = errors.Select(error => new ApiError{Message = error.Description, Slug = error.Code}).ToList()
        };
    }
    
    public static bool HasValue<T>(this ErrorOr<T> errorOr) => !errorOr.IsError;
}