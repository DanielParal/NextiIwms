using MediatR;
using Nexticz.Module.Mmo.SharedKernel.MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.NotificationCollectors;

internal class SettingsNotificationCollector(IMediator mediator) : NotificationCollector(mediator), ISettingsNotificationCollector;