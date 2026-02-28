using MediatR;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUsersBySigningDeviceCode;

internal record GetUsersBySigningDeviceCodeQuery(string SigningDeviceCode) : IRequest<User[]>; 