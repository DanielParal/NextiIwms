using MediatR;
using Nexticz.Module.Sign.SharedKernel.MediatR;

namespace Nexticz.Module.Sign.Settings.Application.NotificationCollectors;

internal class SettingsNotificationCollector(IMediator mediator) : NotificationCollector(mediator), ISettingsNotificationCollector;