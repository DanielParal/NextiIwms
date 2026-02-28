using Nexticz.Module.Vh.Contracts.LoadingDevices;
using Nexticz.Module.Vh.Domain.LoadingDevices;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.LoadingDevices.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface ILoadingDevicesRepository
{
    Task<LoadingDevice?> GetLoadingDeviceByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<LoadingDevice?> GetLoadingDeviceByDeviceKeyAsync(Guid deviceKey, CancellationToken cancellationToken);
    Task<LoadingDeviceResponse?> GetLoadingDeviceResponseByDeviceKeyAsync(Guid deviceKey, CancellationToken cancellationToken);
    Task<LoadingDeviceResponse?> GetLoadingDeviceResponseByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<FilteredResult> GetLoadingDevicesAsync(LoadingDevicesFilteringParams filteringParams,
        CancellationToken cancellationToken);
}