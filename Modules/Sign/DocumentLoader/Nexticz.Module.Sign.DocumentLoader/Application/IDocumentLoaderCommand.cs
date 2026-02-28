using MediatR;

namespace Nexticz.Module.Sign.DocumentLoader.Application;

internal interface IDocumentLoaderCommand<out TResponse> : IRequest<TResponse>;