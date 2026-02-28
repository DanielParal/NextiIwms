using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailTemplates.Queries.GetEmailTemplates;

internal record GetEmailTemplatesQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<EmailTemplate>>;