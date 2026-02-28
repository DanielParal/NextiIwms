using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailTemplates.Queries.GetEmailTemplates;

internal class GetEmailTemplatesQueryHandler
    (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetEmailTemplatesQuery, FilteredResult<EmailTemplate>>
{
    public async Task<FilteredResult<EmailTemplate>> Handle(GetEmailTemplatesQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<EmailTemplate>(request.FilteringParams, cancellationToken);
    }
}