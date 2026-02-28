using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.NotificationCollectors;
using Nexticz.Module.Mmo.SharedKernel.MediatR;

namespace Nexticz.Module.Mmo.Reporting.Application.PipelineBehaviors;

internal class ReportingPostCommandBehavior<TRequest, TResponse>(
    IReportingUnitOfWork unitOfWork,
    IReportingNotificationCollector notificationCollector) 
    : PostCommandBehavior<TRequest, TResponse, IReportingCommand<TResponse>>(unitOfWork, notificationCollector) where TRequest : IRequest<TResponse>
{
    
}