using MediatR;
using Nexticz.Module.Mmo.Drying.Application.Interfaces;
using Nexticz.Module.Mmo.Drying.Application.NotificationCollectors;
using Nexticz.Module.Mmo.SharedKernel.MediatR;

namespace Nexticz.Module.Mmo.Drying.Application.PipelineBehaviors;

internal class DryingPostCommandBehavior<TRequest, TResponse>(
    IDryingUnitOfWork planningUnitOfWork,
    IDryingNotificationCollector notificationCollector) 
    : PostCommandBehavior<TRequest, TResponse, IDryingCommand<TResponse>>(planningUnitOfWork, notificationCollector) where TRequest : IRequest<TResponse>
{
    
}