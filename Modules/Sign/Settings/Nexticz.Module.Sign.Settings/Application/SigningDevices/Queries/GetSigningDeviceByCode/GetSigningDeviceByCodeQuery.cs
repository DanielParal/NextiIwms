using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDeviceByCode;

internal record GetSigningDeviceByCodeQuery(string Code) : IRequest<ErrorOr<SigningDevice>>; 