using MediatR;
using ErrorOr;
using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;
using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates.Queries;
using Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Queries.GetDocumentTemplateByCode;

namespace Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Queries.GetDocumentTemplateResponseByCode;

internal class GetDocumentTemplateResponseByCodeQueryHandler(ISender sender) : IRequestHandler<GetDocumentTemplateResponseByCodeQuery, ErrorOr<DocumentTemplateResponse>>
{
    public async Task<ErrorOr<DocumentTemplateResponse>> Handle(GetDocumentTemplateResponseByCodeQuery request, CancellationToken cancellationToken)
    {
        var documentTemplate = await sender.Send(new GetDocumentTemplateByCodeQuery(request.Code), cancellationToken);
        
        if (documentTemplate.IsError)
            return documentTemplate.Errors;
        
        return DocumentTemplateResponseFactory.Create(documentTemplate.Value);
    }
}