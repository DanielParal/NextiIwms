using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Domain.UserAggregate;

namespace Nexticz.Module.Auth.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<ErrorOr<User>>;