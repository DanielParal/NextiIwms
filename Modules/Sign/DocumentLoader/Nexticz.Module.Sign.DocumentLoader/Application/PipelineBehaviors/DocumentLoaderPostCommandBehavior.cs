using MediatR;
using Nexticz.Module.Sign.DocumentLoader.Application.Interfaces;
using Nexticz.Module.Sign.DocumentLoader.Application.NotificationCollectors;
using Nexticz.Module.Sign.SharedKernel.MediatR;

namespace Nexticz.Module.Sign.DocumentLoader.Application.PipelineBehaviors;

internal class DocumentLoaderPostCommandBehavior<TRequest, TResponse>(
    IDocumentLoaderUnitOfWork documentLoaderUnitOfWork,
    IDocumentLoaderNotificationCollector notificationCollector) 
    : PostCommandBehavior<TRequest, TResponse, IDocumentLoaderCommand<TResponse>>(documentLoaderUnitOfWork, notificationCollector) where TRequest : IRequest<TResponse>
{
    
}