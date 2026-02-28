using MediatR;
using Nexticz.Lib.Shared.MediatR;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Application.NotificationCollectors;

namespace Nexticz.Module.Cuzk.Application.PipelineBehaviors;

internal class CuzkPostCommandBehavior<TRequest, TResponse>(
    ICuzkUnitOfWork unitOfWork,
    INotificationCollector notificationCollector) 
    : MediatRPostCommandBehavior<TRequest, TResponse, ICuzkCommand<TResponse>>(unitOfWork, notificationCollector) where TRequest : IRequest<TResponse>;