using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.LoadingDevices.Commands.DeleteLoadingDevice;

public class DeleteLoadingDeviceCommand : IRequest<ErrorOr<Deleted>>
{
    public required Guid Id { get; set; }
}