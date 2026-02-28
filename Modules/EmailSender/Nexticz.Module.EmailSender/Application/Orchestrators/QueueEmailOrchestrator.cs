using MediatR;
using Nexticz.Module.EmailSender.Contracts;
using Nexticz.Module.EmailSender.Application.EmailMessages.Commands.ScheduleEmailMessage;

namespace Nexticz.Module.EmailSender.Application.Orchestrators;

internal class QueueEmailOrchestrator(ISender sender) : IQueueEmailOrchestrator
{
    public async Task QueueEmailAsync(QueueEmailMessage queueEmailMessage)
    {
        await sender.Send(new ScheduleEmailMessageCommand(queueEmailMessage), CancellationToken.None);
    }
}