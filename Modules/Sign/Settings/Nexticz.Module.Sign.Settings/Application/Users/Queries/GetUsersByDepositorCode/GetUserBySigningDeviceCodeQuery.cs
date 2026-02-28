using MediatR;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUsersByDepositorCode;

internal record GetUsersByDepositorCodeQuery(string DepositorCode) : IRequest<User[]>; 