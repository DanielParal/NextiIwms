using MediatR;
using Nexticz.Lib.Shared.MediatR;

namespace Nexticz.Module.Portal.Application.NotificationCollectors;

internal class NotificationCollector(IMediator mediator) : MediatRNotificationCollector(mediator), INotificationCollector;