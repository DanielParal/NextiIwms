using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetSigningDeviceByCode;

internal record GetSigningDeviceByCodeQuery(string Code) : IRequest<ErrorOr<SigningDevice>>;