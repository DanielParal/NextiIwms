using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Lib.Shared.Extensions;


namespace Nexticz.Lib.Shared.Helpers;

public static class ResultsHelper
{
    public static IResult Problem(Error error)
    {
        return error.Type switch
        {
            ErrorType.Validation => Results.UnprocessableEntity(error.MapToErrorResponse()),
            ErrorType.Failure => Results.UnprocessableEntity(error.MapToErrorResponse()),
            ErrorType.Unauthorized => Results.Unauthorized(),
            ErrorType.NotFound => Results.NotFound(error.MapToErrorResponse()),
            _ => Results.Empty
        };
    }
    
    public static IResult Problem(List<Error> error)
    {
        return error.First().Type switch
        {
            ErrorType.Validation => Results.UnprocessableEntity(error.MapToErrorResponse()),
            ErrorType.Failure => Results.UnprocessableEntity(error.MapToErrorResponse()),
            ErrorType.Unauthorized => Results.Unauthorized(),
            ErrorType.NotFound => Results.NotFound(error.MapToErrorResponse()),
            _ => Results.Empty
        };
    }
}