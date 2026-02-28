using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Users.Queries.GetUserByUserName;

internal class GetUserByUserNameQueryHandler(IUserReadOnlyRepository userReadOnlyRepository) : IRequestHandler<GetUserByUserNameQuery, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
    {
        var mmoUser = await userReadOnlyRepository.GetByUserNameAsync(request.UserName, cancellationToken);

        if (mmoUser is null)
            return UserErrors.UserNotFound;
        
        return mmoUser;
    }
}