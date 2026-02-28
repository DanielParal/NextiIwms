using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.Users;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.CanUserManageSigningDevice;

internal record CanUserManageSigningDeviceQuery(SigningDevice SigningDevice, UserResponse User) : IRequest<bool>;