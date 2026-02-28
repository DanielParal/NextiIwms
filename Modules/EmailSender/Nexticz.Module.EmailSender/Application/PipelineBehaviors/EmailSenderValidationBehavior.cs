using ErrorOr;
using FluentValidation;
using MediatR;
using Nexticz.Lib.Shared.MediatR;

namespace Nexticz.Module.EmailSender.Application.PipelineBehaviors;

public class EmailSenderValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null) 
    : ValidationBehavior<TRequest, TResponse>(validator)
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr;