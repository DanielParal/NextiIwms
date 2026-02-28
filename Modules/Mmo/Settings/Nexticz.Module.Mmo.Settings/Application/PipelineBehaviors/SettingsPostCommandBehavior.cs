using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.NotificationCollectors;
using Nexticz.Module.Mmo.SharedKernel.MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.PipelineBehaviors;

internal class SettingsPostCommandBehavior<TRequest, TResponse>(
    ISettingsUnitOfWork settingsUnitOfWork,
    ISettingsNotificationCollector notificationCollector) 
    : PostCommandBehavior<TRequest, TResponse, ISettingsCommand<TResponse>>(settingsUnitOfWork, notificationCollector) where TRequest : IRequest<TResponse>
{
    
}