using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUsersByDepositorGroupCode;

internal class GetUsersByDepositorGroupCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetUsersByDepositorGroupCodeQuery, User[]>
{
    public async Task<User[]> Handle(GetUsersByDepositorGroupCodeQuery request, CancellationToken cancellationToken)
    {
        var upperCode = request.DepositorGroupCode.ToUpperInvariant();
        var users = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<User>(
                x => x.DepositorGroupCodes.Any(y => y == upperCode), 
                cancellationToken);
        
        return users.ToArray();
    }
}