using MediatR;
using Nexticz.Module.Mmo.SharedKernel.MediatR;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Application.NotificationCollectors;

namespace Nexticz.Module.Mmo.Washing.Application.PipelineBehaviors;

internal class WashingPostCommandBehavior<TRequest, TResponse>(
    IWashingUnitOfWork washingUnitOfWork,
    IWashingNotificationCollector notificationCollector) 
    : PostCommandBehavior<TRequest, TResponse, IWashingCommand<TResponse>>(washingUnitOfWork, notificationCollector) where TRequest : IRequest<TResponse>
{
    
}