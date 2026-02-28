using MediatR;

namespace Nexticz.Module.Sign.DocumentManager.Application;

internal interface IDocumentManagerCommand<out TResponse> : IRequest<TResponse>;