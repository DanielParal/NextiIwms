using MediatR;
using Nexticz.Lib.Shared.MediatR;

namespace Nexticz.Module.Sign.SharedKernel.MediatR;

public class NotificationCollector(IMediator mediator) : MediatRNotificationCollector(mediator), INotificationCollector;