using MediatR;
using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Queries.GetEmailConfigurationById;

internal record GetEmailConfigurationByIdQuery(Guid Id) : IRequest<ErrorOr<EmailConfiguration>>;