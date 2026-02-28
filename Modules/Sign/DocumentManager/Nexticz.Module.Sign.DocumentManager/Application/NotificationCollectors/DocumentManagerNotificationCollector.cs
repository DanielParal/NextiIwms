using MediatR;
using Nexticz.Module.Sign.SharedKernel.MediatR;

namespace Nexticz.Module.Sign.DocumentManager.Application.NotificationCollectors;

internal class DocumentManagerNotificationCollector(IMediator mediator) : NotificationCollector(mediator), IDocumentManagerNotificationCollector;