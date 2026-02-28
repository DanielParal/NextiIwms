using MediatR;
using Nexticz.Lib.Shared.MediatR;
using Nexticz.Module.Portal.Application.Interfaces;
using Nexticz.Module.Portal.Application.NotificationCollectors;

namespace Nexticz.Module.Portal.Application.PipelineBehaviors;

internal class PortalPostCommandBehavior<TRequest, TResponse>(
    IPortalUnitOfWork unitOfWork,
    INotificationCollector notificationCollector) 
    : MediatRPostCommandBehavior<TRequest, TResponse, IPortalCommand<TResponse>>(unitOfWork, notificationCollector) where TRequest : IRequest<TResponse>;