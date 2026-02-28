using MediatR;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Application.NotificationCollectors;
using Nexticz.Module.Mmo.SharedKernel.MediatR;

namespace Nexticz.Module.Mmo.Planning.Application.PipelineBehaviors;

internal class PlanningPostCommandBehavior<TRequest, TResponse>(
    IPlanningUnitOfWork planningUnitOfWork,
    IPlanningNotificationCollector notificationCollector) 
    : PostCommandBehavior<TRequest, TResponse, IPlanningCommand<TResponse>>(planningUnitOfWork, notificationCollector) where TRequest : IRequest<TResponse>
{
    
}