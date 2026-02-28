using MediatR;
using Nexticz.Lib.Shared.MediatR;
using Nexticz.Module.Notifications.Application.Interfaces;
using Nexticz.Module.Notifications.Application.NotificationCollectors;

namespace Nexticz.Module.Notifications.Application.PipelineBehaviors;

internal class NotificationPostCommandBehavior<TRequest, TResponse>(
    INotificationUnitOfWork unitOfWork,
    INotificationCollector notificationCollector) 
    : MediatRPostCommandBehavior<TRequest, TResponse, INotificationCommand<TResponse>>(unitOfWork, notificationCollector) where TRequest : IRequest<TResponse>
{
    
}