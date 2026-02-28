using ErrorOr;
using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.EmailTemplates.Queries;

public record GetEmailTemplateContractByCodeQuery(string Code) : IRequest<ErrorOr<EmailTemplateContract>>;