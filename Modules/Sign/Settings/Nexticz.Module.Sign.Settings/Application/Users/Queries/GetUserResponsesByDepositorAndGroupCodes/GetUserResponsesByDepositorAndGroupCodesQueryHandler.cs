using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.Users;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUserResponsesByDepositorAndGroupCodes;

internal class GetUserResponsesByDepositorAndGroupCodesQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)  : IRequestHandler<GetUserResponsesByDepositorAndGroupCodesQuery, UserResponse[]>
{
    public async Task<UserResponse[]> Handle(GetUserResponsesByDepositorAndGroupCodesQuery request, CancellationToken cancellationToken)
    {
        var upperDepositorCodes = request.DepositorCodes.Select(code => code.ToUpperInvariant()).ToArray();
        var upperDepositorGroupCodes = request.DepositorGroupCodes.Select(code => code.ToUpperInvariant()).ToArray();
        var users = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<User>(
                x => x.DepositorCodes.Any(y => upperDepositorCodes.Contains(y)) || 
                     x.DepositorGroupCodes.Any(y => upperDepositorGroupCodes.Contains(y)), 
                cancellationToken);

        return users.Select(UserResponseFactory.Create).ToArray();
    }
}