using ErrorOr;
using FluentValidation;
using MediatR;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Mmo.Settings.Application.PipelineBehaviors;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validator is null)
        {
            return await next();
        }

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
        {
            return await next();
        }

        var errors = ErrorHelper.ReplaceParametersInValidationResultErrors(validationResult.Errors);

        return (dynamic)errors;
    }
}