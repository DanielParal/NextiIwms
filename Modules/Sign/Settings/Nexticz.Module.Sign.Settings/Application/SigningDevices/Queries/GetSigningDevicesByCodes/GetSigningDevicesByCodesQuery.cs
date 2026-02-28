using MediatR;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDevicesByCodes;

internal record GetSigningDevicesByCodesQuery(string[] Codes) : IRequest<SigningDevice[]>; 