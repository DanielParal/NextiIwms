using MassTransit;
using Nexticz.Lib.Shared.Logging;
using Nexticz.Lib.Shared.MessagePublishers;

namespace Nexticz.Module.Mmo.Washing.Application.MessagePublishers;

public class MessagePublisher(IPublishEndpoint publishEndpoint) : BaseMessagePublisher(publishEndpoint), IMessagePublisher;