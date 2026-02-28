using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.LoadingDevices;

namespace Nexticz.Module.Vh.Application.LoadingDevices.Commands.CreateLoadingDevice;

public class CreateLoadingDeviceCommand : IRequest<ErrorOr<Created>>
{
    public required CreateLoadingDeviceRequest CreateLoadingDeviceRequest { get; set; }
    
}