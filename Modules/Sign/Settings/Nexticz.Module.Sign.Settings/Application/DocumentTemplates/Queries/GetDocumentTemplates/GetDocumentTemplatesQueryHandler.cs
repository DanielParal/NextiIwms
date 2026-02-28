using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Queries.GetDocumentTemplates;

internal class GetDocumentTemplatesQueryHandler
    (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetDocumentTemplatesQuery, FilteredResult<DocumentTemplate>>
{
    public async Task<FilteredResult<DocumentTemplate>> Handle(GetDocumentTemplatesQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<DocumentTemplate>(request.FilteringParams, cancellationToken);
    }
}