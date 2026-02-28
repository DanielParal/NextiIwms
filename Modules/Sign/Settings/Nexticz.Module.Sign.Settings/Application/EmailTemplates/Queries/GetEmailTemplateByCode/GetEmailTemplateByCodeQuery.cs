using MediatR;
using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailTemplates.Queries.GetEmailTemplateByCode;

internal record GetEmailTemplateByCodeQuery(string Code) : IRequest<ErrorOr<EmailTemplate>>;