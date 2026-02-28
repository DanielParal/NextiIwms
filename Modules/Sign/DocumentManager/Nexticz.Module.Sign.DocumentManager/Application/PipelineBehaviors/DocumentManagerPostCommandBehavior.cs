using MediatR;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Application.NotificationCollectors;
using Nexticz.Module.Sign.SharedKernel.MediatR;

namespace Nexticz.Module.Sign.DocumentManager.Application.PipelineBehaviors;

internal class DocumentManagerPostCommandBehavior<TRequest, TResponse>(
    IDocumentManagerUnitOfWork documentManagerUnitOfWork,
    IDocumentManagerNotificationCollector notificationCollector) 
    : PostCommandBehavior<TRequest, TResponse, IDocumentManagerCommand<TResponse>>(documentManagerUnitOfWork, notificationCollector) where TRequest : IRequest<TResponse>
{
    
}