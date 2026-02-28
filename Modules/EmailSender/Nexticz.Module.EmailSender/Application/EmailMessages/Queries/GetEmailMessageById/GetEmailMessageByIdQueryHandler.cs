using MediatR;
using ErrorOr;
using Nexticz.Module.EmailSender.Application.Interfaces;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;

namespace Nexticz.Module.EmailSender.Application.EmailMessages.Queries.GetEmailMessageById;

internal class GetEmailMessageByIdQueryHandler(
    IEmailSenderReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IRequestHandler<GetEmailMessageByIdQuery, ErrorOr<EmailMessage>>
{
    public async Task<ErrorOr<EmailMessage>> Handle(GetEmailMessageByIdQuery request, CancellationToken cancellationToken)
    {
        var emailMessage = await readOnlyEventStoreRepository.GetByIdAsync<EmailMessage>(request.Id, cancellationToken);

        if (emailMessage is null)
            return EmailMessageErrors.EmailMessageNotFound;
        
        return emailMessage;
    }
}