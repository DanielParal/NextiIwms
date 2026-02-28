using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Domain.UserAggregate;

namespace Nexticz.Module.Auth.Application.Users.Queries.GetUserByEmail;

public class GetUserByEmailQueryHandler(IUserReadOnlyRepository userReadOnlyRepository)
    : IRequestHandler<GetUserByEmailQuery, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        var user = await userReadOnlyRepository.GetUserByEmailAsync(query.Email, cancellationToken);

        if (user is null) 
            return UserErrors.ValidationUserWithEmailDoesNotExist;

        return user;
    }
}