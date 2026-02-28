using MediatR;

namespace Nexticz.Module.Mmo.Drying.Application;

internal interface IDryingCommand<out TResponse> : IRequest<TResponse>;