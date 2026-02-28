using MediatR;

namespace Nexticz.Module.EmailSender.Application;

internal interface IEmailSenderCommand<out TResponse> : IRequest<TResponse>;