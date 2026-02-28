using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Domain.UserAggregate;

namespace Nexticz.Module.Auth.Application.Users.Queries.GetUserByUsername;

public record GetUserByUsernameQuery(string Username) : IRequest<ErrorOr<User>>;