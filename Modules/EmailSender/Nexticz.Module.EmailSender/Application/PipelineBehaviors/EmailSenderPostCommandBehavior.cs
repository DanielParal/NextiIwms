using MediatR;
using Nexticz.Lib.Shared.MediatR;
using Nexticz.Module.EmailSender.Application.Interfaces;
using Nexticz.Module.EmailSender.Application.NotificationCollectors;

namespace Nexticz.Module.EmailSender.Application.PipelineBehaviors;

internal class EmailSenderPostCommandBehavior<TRequest, TResponse>(
    IEmailSenderUnitOfWork unitOfWork,
    IEmailSenderNotificationCollector notificationCollector) 
    : MediatRPostCommandBehavior<TRequest, TResponse, IEmailSenderCommand<TResponse>>(unitOfWork, notificationCollector) where TRequest : IRequest<TResponse>
{
    
}