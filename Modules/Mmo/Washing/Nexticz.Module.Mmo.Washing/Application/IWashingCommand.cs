using MediatR;

namespace Nexticz.Module.Mmo.Washing.Application;

internal interface IWashingCommand<out TResponse> : IRequest<TResponse>;