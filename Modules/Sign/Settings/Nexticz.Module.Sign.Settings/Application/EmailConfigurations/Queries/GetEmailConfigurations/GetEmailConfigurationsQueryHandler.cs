using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Queries.GetEmailConfigurations;

internal class GetEmailConfigurationsQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)  : IRequestHandler<GetEmailConfigurationsQuery, FilteredResult<EmailConfiguration>>
{
    public async Task<FilteredResult<EmailConfiguration>> Handle(GetEmailConfigurationsQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<EmailConfiguration>(request.FilteringParams, cancellationToken);
    }
}