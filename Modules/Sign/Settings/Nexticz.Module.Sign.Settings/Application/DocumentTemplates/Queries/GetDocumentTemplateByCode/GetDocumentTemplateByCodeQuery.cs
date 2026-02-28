using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Queries.GetDocumentTemplateByCode;

internal record GetDocumentTemplateByCodeQuery(string Code) : IRequest<ErrorOr<DocumentTemplate>>; 