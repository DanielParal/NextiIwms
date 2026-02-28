using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Queries.GetEmailConfigurations;

internal record GetEmailConfigurationsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<EmailConfiguration>>;