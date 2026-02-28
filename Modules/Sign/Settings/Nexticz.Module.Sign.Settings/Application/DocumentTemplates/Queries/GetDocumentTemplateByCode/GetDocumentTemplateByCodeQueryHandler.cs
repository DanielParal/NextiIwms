using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Queries.GetDocumentTemplateByCode;

internal class GetDocumentTemplateByCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetDocumentTemplateByCodeQuery, ErrorOr<DocumentTemplate>>
{
    public async Task<ErrorOr<DocumentTemplate>> Handle(GetDocumentTemplateByCodeQuery request, CancellationToken cancellationToken)
    {
        var documentTemplate = await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<DocumentTemplate>(
                x => x.Code.Equals(request.Code, StringComparison.InvariantCultureIgnoreCase), 
                cancellationToken);

        if (documentTemplate is null)
            return DocumentTemplateErrors.CodeNotFound;
        
        return documentTemplate;
    }
}