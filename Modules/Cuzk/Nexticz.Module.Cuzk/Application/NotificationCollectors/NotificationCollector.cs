using MediatR;
using Nexticz.Lib.Shared.MediatR;

namespace Nexticz.Module.Cuzk.Application.NotificationCollectors;

internal class NotificationCollector(IMediator mediator) : MediatRNotificationCollector(mediator), INotificationCollector;