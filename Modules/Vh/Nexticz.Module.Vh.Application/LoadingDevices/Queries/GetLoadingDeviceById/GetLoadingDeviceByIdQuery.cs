using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.LoadingDevices;

namespace Nexticz.Module.Vh.Application.LoadingDevices.Queries.GetLoadingDeviceById;

public class GetLoadingDeviceByIdQuery : IRequest<ErrorOr<LoadingDeviceResponse>>
{
    public required Guid Id { get; set; }
}