using MediatR;

namespace Nexticz.Module.Mmo.Planning.Application;

internal interface IPlanningCommand<out TResponse> : IRequest<TResponse>;