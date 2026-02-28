using MediatR;

namespace Nexticz.Module.Cuzk.Application;

internal interface ICuzkCommand<out TResponse> : IRequest<TResponse>;