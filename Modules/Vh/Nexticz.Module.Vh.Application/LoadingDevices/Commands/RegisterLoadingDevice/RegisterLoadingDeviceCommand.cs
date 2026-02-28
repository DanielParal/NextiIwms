using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.LoadingDevices.Commands.RegisterLoadingDevice;

public class RegisterLoadingDeviceCommand : IRequest<ErrorOr<Success>>
{
    public required Guid DeviceKey { get; set; }
}