using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadingDevices;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.LoadingDevices.Commands.DeleteLoadingDevice;

public class DeleteLoadingDeviceCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteLoadingDeviceCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteLoadingDeviceCommand command, CancellationToken cancellationToken)
    {
        var loadingDevice =
            await unitOfWork.LoadingDevicesRepository.GetLoadingDeviceByIdAsync(command.Id, cancellationToken);

        if (loadingDevice is null) return LoadingDeviceErrors.LoadingDeviceWithIdDoesnotExist;

        unitOfWork.Remove(loadingDevice);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;
    }
}