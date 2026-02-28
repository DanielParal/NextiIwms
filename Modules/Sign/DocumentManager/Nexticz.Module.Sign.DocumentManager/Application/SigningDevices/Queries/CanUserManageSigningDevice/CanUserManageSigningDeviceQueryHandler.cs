using MediatR;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.CanUserManageSigningDevice;

internal class CanUserManageSigningDeviceQueryHandler() : IRequestHandler<CanUserManageSigningDeviceQuery, bool>
{
    public Task<bool> Handle(CanUserManageSigningDeviceQuery request, CancellationToken cancellationToken)
    {
        return request.User.SigningDeviceCodes.Contains(request.SigningDevice.Code)
            ? Task.FromResult(true)
            : Task.FromResult(false);
    }
}