using ErrorOr;
using FluentValidation;
using MediatR;
using Nexticz.Lib.Shared.MediatR;

namespace Nexticz.Module.Sign.DocumentManager.Application.PipelineBehaviors;

public class DocumentManagerValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null) 
    : ValidationBehavior<TRequest, TResponse>(validator)
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr;