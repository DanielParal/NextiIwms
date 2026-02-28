using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Domain.UserAggregate;

namespace Nexticz.Module.Auth.Application.Users.Queries.GetUserByEmail;

public record GetUserByEmailQuery(string Email) : IRequest<ErrorOr<User>>;