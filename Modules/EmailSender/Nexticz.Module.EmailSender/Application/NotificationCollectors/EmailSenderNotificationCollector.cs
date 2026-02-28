using MediatR;
using Nexticz.Lib.Shared.MediatR;

namespace Nexticz.Module.EmailSender.Application.NotificationCollectors;

internal class EmailSenderNotificationCollector(IMediator mediator) : MediatRNotificationCollector(mediator), IEmailSenderNotificationCollector;