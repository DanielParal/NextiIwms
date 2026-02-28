using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.LoadingDevices;

namespace Nexticz.Module.Vh.Application.LoadingDevices.Queries.GetLaodingDeviceByDeviceKey;

public class GetLoadingDeviceByDeviceKeyQuery : IRequest<ErrorOr<LoadingDeviceResponse>>
{
    public required Guid DeviceKey { get; set; }
}