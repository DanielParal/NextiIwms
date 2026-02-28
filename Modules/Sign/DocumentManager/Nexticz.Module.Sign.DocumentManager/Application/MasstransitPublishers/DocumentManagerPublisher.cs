using MassTransit;
using Nexticz.Lib.Shared.MessagePublishers;

namespace Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers;

internal class DocumentManagerPublisher(IPublishEndpoint publishEndpoint) 
    : BaseMessagePublisher(publishEndpoint), IDocumentManagerPublisher;