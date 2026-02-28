using MediatR;
using Nexticz.Module.Sign.SharedKernel.MediatR;

namespace Nexticz.Module.Sign.DocumentLoader.Application.NotificationCollectors;

internal class DocumentLoaderNotificationCollector(IMediator mediator) : NotificationCollector(mediator), IDocumentLoaderNotificationCollector;