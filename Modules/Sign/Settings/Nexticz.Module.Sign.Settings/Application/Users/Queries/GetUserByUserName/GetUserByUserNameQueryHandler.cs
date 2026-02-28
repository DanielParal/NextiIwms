using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUserByUserName;

internal class GetUserByUserNameQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetUserByUserNameQuery, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
    {
        var user = await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<User>(
                x => x.UserName.Equals(request.UserName, StringComparison.InvariantCultureIgnoreCase), 
                cancellationToken);

        if (user is null)
            return UserErrors.UserNameNotFound;
        
        return user;
    }
}