using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.Users;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUserByUserName;

namespace Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUserResponseByUserName;

internal class GetUserResponseByUserNameQueryHandler(
    ISender sender) 
    : IRequestHandler<GetUserResponseByUserNameQuery, ErrorOr<UserResponse>>
{
    public async Task<ErrorOr<UserResponse>> Handle(GetUserResponseByUserNameQuery request, CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByUserNameQuery(request.UserName), cancellationToken);

        if (user.IsError)
            return user.Errors;
        
        return UserResponseFactory.Create(user.Value);
    }
}