using MediatR;

namespace Nexticz.Module.Notifications.Application;

internal interface INotificationCommand<out TResponse> : IRequest<TResponse>;