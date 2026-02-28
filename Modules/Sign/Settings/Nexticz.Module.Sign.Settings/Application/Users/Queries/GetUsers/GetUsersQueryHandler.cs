using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUsers;

internal class GetUsersQueryHandler
    (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetUsersQuery, FilteredResult<User>>
{
    public async Task<FilteredResult<User>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<User>(request.FilteringParams, cancellationToken);
    }
}