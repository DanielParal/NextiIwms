using MediatR;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDevicesByLocationCode;

internal record GetSigningDevicesByLocationCodeQuery(string LocationCode) : IRequest<SigningDevice[]>; 