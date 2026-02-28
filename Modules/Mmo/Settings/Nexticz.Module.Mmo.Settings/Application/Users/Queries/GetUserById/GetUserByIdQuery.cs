using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Users.Queries.GetUserById;

internal record GetUserByIdQuery(Guid Id) : IRequest<ErrorOr<User>>;