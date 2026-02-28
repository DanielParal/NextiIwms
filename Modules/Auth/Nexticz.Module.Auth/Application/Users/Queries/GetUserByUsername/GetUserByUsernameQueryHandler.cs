using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Domain.UserAggregate;

namespace Nexticz.Module.Auth.Application.Users.Queries.GetUserByUsername;

public class GetUserByUsernameQueryHandler(IUserReadOnlyRepository userReadOnlyRepository)
    : IRequestHandler<GetUserByUsernameQuery, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(GetUserByUsernameQuery query,
        CancellationToken cancellationToken)
    {
        var user = await userReadOnlyRepository.GetUserByUserNameAsync(query.Username, cancellationToken);

        if (user is null) 
            return UserErrors.ValidationUserWithUsernameDoesNotExist;

        return user;
    }
}