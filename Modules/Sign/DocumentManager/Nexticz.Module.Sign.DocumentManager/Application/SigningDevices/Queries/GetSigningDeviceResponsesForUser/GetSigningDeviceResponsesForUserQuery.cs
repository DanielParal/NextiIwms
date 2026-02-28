using MediatR;
using Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetSigningDeviceResponsesForUser;

internal record GetSigningDeviceResponsesForUserQuery() : IRequest<SigningDeviceResponse[]>;