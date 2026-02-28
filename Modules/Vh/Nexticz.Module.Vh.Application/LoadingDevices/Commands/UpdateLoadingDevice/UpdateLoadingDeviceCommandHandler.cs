using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadingDevices;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.LoadingDevices.Commands.UpdateLoadingDevice;

public class UpdateLoadingDeviceCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateLoadingDeviceCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateLoadingDeviceCommand command, CancellationToken cancellationToken)
    {
        var loadingDevice =
            await unitOfWork.LoadingDevicesRepository.GetLoadingDeviceByDeviceKeyAsync(command.DeviceKey, cancellationToken);

        if (loadingDevice is null)
        {
            return LoadingDeviceErrors.LoadingDeviceWithIdDoesnotExist;
        }
        
        loadingDevice.Name = command.UpdateLoadingDeviceRequest.Name;
        loadingDevice.BlockedFrom = command.UpdateLoadingDeviceRequest.BlockedFrom;

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;
    }
}