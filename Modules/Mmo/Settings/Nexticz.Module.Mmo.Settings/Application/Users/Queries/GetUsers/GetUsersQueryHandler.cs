using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Users.Queries.GetUsers;

internal class GetUsersQueryHandler(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetUsersQuery, FilteredResult<User>>
{
    public async Task<FilteredResult<User>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<User>(request.FilteringParams, cancellationToken);
    }
}