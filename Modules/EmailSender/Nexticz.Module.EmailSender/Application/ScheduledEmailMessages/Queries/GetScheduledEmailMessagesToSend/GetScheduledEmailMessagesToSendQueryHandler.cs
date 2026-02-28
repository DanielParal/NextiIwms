using MediatR;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.EmailSender.Application.Interfaces;
using Nexticz.Module.EmailSender.Domain.Views;

namespace Nexticz.Module.EmailSender.Application.ScheduledEmailMessages.Queries.GetScheduledEmailMessagesToSend;

internal class GetScheduledEmailMessagesToSendQueryHandler(
    IEmailSenderReadOnlyEventStoreRepository readOnlyEventStoreRepository,
    IClock clock) : IRequestHandler<GetScheduledEmailMessagesToSendQuery, ScheduledEmailMessageView[]>
{
    public async Task<ScheduledEmailMessageView[]> Handle(GetScheduledEmailMessagesToSendQuery request, CancellationToken cancellationToken)
    {
        var scheduledEmailMessages = 
            await readOnlyEventStoreRepository.GetAllByConditionAsync<ScheduledEmailMessageView>(
                x => x.ScheduledToBeSentAt < clock.UtcNowOffset, 
                cancellationToken);
        
        return scheduledEmailMessages.ToArray();
    }
}