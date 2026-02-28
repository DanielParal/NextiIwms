using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Users.Queries.GetUserByUserName;

internal record GetUserByUserNameQuery(string UserName) : IRequest<ErrorOr<User>>;