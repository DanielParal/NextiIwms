using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadingDevices;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.LoadingDevices.Commands.CreateLoadingDevice;

public class CreateLoadingDeviceCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateLoadingDeviceCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateLoadingDeviceCommand command, CancellationToken cancellationToken)
    {
        var loadingDevice = new LoadingDevice
        {
            Name = command.CreateLoadingDeviceRequest.Name
        };

        unitOfWork.Add(loadingDevice);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}