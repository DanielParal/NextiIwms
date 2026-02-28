using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.NotificationCollectors;
using Nexticz.Module.Sign.SharedKernel.MediatR;

namespace Nexticz.Module.Sign.Settings.Application.PipelineBehaviors;

internal class SettingsPostCommandBehavior<TRequest, TResponse>(
    ISettingsUnitOfWork settingsUnitOfWork,
    ISettingsNotificationCollector notificationCollector) 
    : PostCommandBehavior<TRequest, TResponse, ISettingsCommand<TResponse>>(settingsUnitOfWork, notificationCollector) where TRequest : IRequest<TResponse>
{
    
}