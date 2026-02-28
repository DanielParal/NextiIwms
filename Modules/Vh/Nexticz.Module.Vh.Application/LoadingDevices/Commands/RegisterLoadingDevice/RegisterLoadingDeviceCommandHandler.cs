using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadingDevices;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.LoadingDevices.Commands.RegisterLoadingDevice;

public class RegisterLoadingDeviceCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RegisterLoadingDeviceCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(RegisterLoadingDeviceCommand command, CancellationToken cancellationToken)
    {
        var loadingDevice =
            await unitOfWork.LoadingDevicesRepository.GetLoadingDeviceByDeviceKeyAsync(command.DeviceKey,
                cancellationToken);

        if (loadingDevice is null)
        {
            return LoadingDeviceErrors.LoadingDeviceWithIdDoesnotExist;
        }

        if (loadingDevice.UsedFrom is not null)
        {
            return LoadingDeviceErrors.LoadingDeviceCanNotBeRegisteredError;
        }
        
        loadingDevice.UsedFrom = DateTime.UtcNow;

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success;throw new NotImplementedException();
    }
}