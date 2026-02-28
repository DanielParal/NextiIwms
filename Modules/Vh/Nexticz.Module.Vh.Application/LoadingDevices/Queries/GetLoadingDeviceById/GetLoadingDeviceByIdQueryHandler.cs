using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.LoadingDevices;
using Nexticz.Module.Vh.Domain.LoadingDevices;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.LoadingDevices.Queries.GetLoadingDeviceById;

public class GetLoadingDeviceByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLoadingDeviceByIdQuery, ErrorOr<LoadingDeviceResponse>>
{
    public async Task<ErrorOr<LoadingDeviceResponse>> Handle(GetLoadingDeviceByIdQuery query, CancellationToken cancellationToken)
    {
        var loadingDevice =
            await unitOfWork.LoadingDevicesRepository.GetLoadingDeviceResponseByIdAsync(query.Id, cancellationToken);

        if (loadingDevice is null)
        {
            return LoadingDeviceErrors.LoadingDeviceWithIdDoesnotExist;
        }

        return loadingDevice;
    }
}