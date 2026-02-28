using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUsersByDepositorCode;

internal class GetUsersByDepositorCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetUsersByDepositorCodeQuery, User[]>
{
    public async Task<User[]> Handle(GetUsersByDepositorCodeQuery request, CancellationToken cancellationToken)
    {
        var upperCode = request.DepositorCode.ToUpperInvariant();
        var users = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<User>(
                x => x.DepositorCodes.Any(y => y == upperCode), 
                cancellationToken);
        
        return users.ToArray();
    }
}