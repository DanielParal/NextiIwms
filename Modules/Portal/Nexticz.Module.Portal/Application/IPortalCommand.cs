using MediatR;

namespace Nexticz.Module.Portal.Application;

internal interface IPortalCommand<out TResponse> : IRequest<TResponse>;