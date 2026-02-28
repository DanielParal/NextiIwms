using MediatR;
using Nexticz.Lib.Shared.MediatR;

namespace Nexticz.Module.Mmo.SharedKernel.MediatR;

public class NotificationCollector(IMediator mediator) : MediatRNotificationCollector(mediator), INotificationCollector;