using MediatR;
using Nexticz.Module.Mmo.SharedKernel.MediatR;

namespace Nexticz.Module.Mmo.Washing.Application.NotificationCollectors;

internal class WashingNotificationCollector(IMediator mediator) : NotificationCollector(mediator), IWashingNotificationCollector;