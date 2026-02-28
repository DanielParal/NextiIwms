using ErrorOr;
using FluentValidation.Results;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Lib.Shared.Helpers;

public static class ErrorHelper
{
    public static List<Error> ReplaceParametersInValidationResultErrors(List<ValidationFailure> validatorResultErrors)
    {
        var errors = new List<Error>();
        foreach (var validatorResultError in validatorResultErrors)
        {
            if (validatorResultError.CustomState is CustomErrorState { Error: Error error, Parameters: object?[] parameters })
            {
                errors.Add( Error.Validation(code: error.Code, description: string.Format(error.Description, parameters)));
            }
        }

        return errors;
    }
}