using MediatR;
using Nexticz.Module.Mmo.SharedKernel.MediatR;

namespace Nexticz.Module.Mmo.Planning.Application.NotificationCollectors;

internal class PlanningNotificationCollector(IMediator mediator) : NotificationCollector(mediator), IPlanningNotificationCollector;