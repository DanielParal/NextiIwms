using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Application.Emails.Queries.GetSentEmails;

internal class GetSentEmailsQueryHandler
    (IDocumentManagerReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetSentEmailsQuery, FilteredResult<SentEmailView>>
{
    public async Task<FilteredResult<SentEmailView>> Handle(GetSentEmailsQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<SentEmailView>(request.FilteringParams, cancellationToken);
    }
}