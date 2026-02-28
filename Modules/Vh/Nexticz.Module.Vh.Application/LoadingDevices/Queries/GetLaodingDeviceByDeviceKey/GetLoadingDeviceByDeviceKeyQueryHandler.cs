using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.LoadingDevices;
using Nexticz.Module.Vh.Domain.LoadingDevices;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.LoadingDevices.Queries.GetLaodingDeviceByDeviceKey;

public class GetLoadingDeviceByDeviceKeyQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLoadingDeviceByDeviceKeyQuery, ErrorOr<LoadingDeviceResponse>>
{
    public async Task<ErrorOr<LoadingDeviceResponse>> Handle(GetLoadingDeviceByDeviceKeyQuery query, CancellationToken cancellationToken)
    {
        var loadingDevice =
            await unitOfWork.LoadingDevicesRepository.GetLoadingDeviceResponseByDeviceKeyAsync(query.DeviceKey,
                cancellationToken);

        if (loadingDevice is null)
        {
            return LoadingDeviceErrors.LoadingDeviceWithIdDoesnotExist;
        }

        return loadingDevice;
    }
}