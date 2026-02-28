using MediatR;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUsersByDepositorGroupCode;

internal record GetUsersByDepositorGroupCodeQuery(string DepositorGroupCode) : IRequest<User[]>; 