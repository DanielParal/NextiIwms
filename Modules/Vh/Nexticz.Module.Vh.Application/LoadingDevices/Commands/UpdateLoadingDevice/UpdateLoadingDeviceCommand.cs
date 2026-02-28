using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.LoadingDevices;

namespace Nexticz.Module.Vh.Application.LoadingDevices.Commands.UpdateLoadingDevice;

public class UpdateLoadingDeviceCommand : IRequest<ErrorOr<Updated>>
{
    public required Guid DeviceKey { get; set; }
    public required UpdateLoadingDeviceRequest UpdateLoadingDeviceRequest { get; set; }
}