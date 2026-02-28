using MediatR;
using Nexticz.Module.Mmo.SharedKernel.MediatR;

namespace Nexticz.Module.Mmo.Drying.Application.NotificationCollectors;

internal class DryingNotificationCollector(IMediator mediator) : NotificationCollector(mediator), IDryingNotificationCollector;