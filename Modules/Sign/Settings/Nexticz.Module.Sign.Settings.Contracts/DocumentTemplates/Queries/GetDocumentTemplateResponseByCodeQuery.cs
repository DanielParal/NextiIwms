using ErrorOr;
using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates.Queries;

public record GetDocumentTemplateResponseByCodeQuery(string Code) : IRequest<ErrorOr<DocumentTemplateResponse>>;