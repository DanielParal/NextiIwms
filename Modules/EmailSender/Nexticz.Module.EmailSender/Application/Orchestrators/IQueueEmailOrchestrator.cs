using Nexticz.Module.EmailSender.Contracts;

namespace Nexticz.Module.EmailSender.Application.Orchestrators;

internal interface IQueueEmailOrchestrator
{
    Task QueueEmailAsync(QueueEmailMessage queueEmailMessage);
}