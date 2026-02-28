using ErrorOr;
using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.Users.Queries;

public record GetUserResponseByUserNameQuery(string UserName) : IRequest<ErrorOr<UserResponse>>; 