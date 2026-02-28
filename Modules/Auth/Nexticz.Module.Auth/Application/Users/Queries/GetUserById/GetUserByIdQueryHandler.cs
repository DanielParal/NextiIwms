using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Domain.UserAggregate;

namespace Nexticz.Module.Auth.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(IUserReadOnlyRepository userReadOnlyRepository)
    : IRequestHandler<GetUserByIdQuery, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await userReadOnlyRepository.GetUserByIdAsync(query.Id, cancellationToken);

        if (user is null) 
            return UserErrors.ValidationUserWithIdDoesNotExist;

        return user;
    }
}