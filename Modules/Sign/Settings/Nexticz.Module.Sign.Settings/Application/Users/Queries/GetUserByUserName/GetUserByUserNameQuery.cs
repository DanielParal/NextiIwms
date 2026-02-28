using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUserByUserName;

internal record GetUserByUserNameQuery(string UserName) : IRequest<ErrorOr<User>>; 