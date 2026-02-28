using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Queries.GetDocumentTemplates;

internal record GetDocumentTemplatesQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<DocumentTemplate>>;