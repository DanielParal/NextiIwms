using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.LoadingDevices.Common.Models;
using Nexticz.Module.Vh.Contracts.LoadingDevices;
using Nexticz.Module.Vh.Domain.LoadingDevices;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.LoadingDevices.Persistence;

public class LoadingDevicesRepository(DataContext context) : ILoadingDevicesRepository
{
    public async Task<LoadingDevice?> GetLoadingDeviceByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.LoadingDevices
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<LoadingDevice?> GetLoadingDeviceByDeviceKeyAsync(Guid deviceKey, CancellationToken cancellationToken)
    {
        return await context.LoadingDevices
            .Where(x => x.DeviceKey == deviceKey)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<LoadingDeviceResponse?> GetLoadingDeviceResponseByDeviceKeyAsync(Guid deviceKey, CancellationToken cancellationToken)
    {
        return await context.LoadingDevices
            .Where(x => x.DeviceKey == deviceKey)
            .Select(x => new LoadingDeviceResponse
            {
                Id = x.Id,
                DeviceKey = x.DeviceKey,
                Name = x.Name,
                LastActivityWorkerCodeWms = x.LastActivityWorkerCodeWms,
                BlockedFrom = x.BlockedFrom,
                UsedFrom = x.UsedFrom,
                LastActivity = x.LastActivity
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<LoadingDeviceResponse?> GetLoadingDeviceResponseByIdAsync(Guid id,
        CancellationToken cancellationToken)
    {
        return await context.LoadingDevices
            .Where(x => x.Id == id)
            .Select(x => new LoadingDeviceResponse
            {
                Id = x.Id,
                DeviceKey = x.DeviceKey,
                Name = x.Name,
                LastActivityWorkerCodeWms = x.LastActivityWorkerCodeWms,
                BlockedFrom = x.BlockedFrom,
                UsedFrom = x.UsedFrom,
                LastActivity = x.LastActivity
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FilteredResult> GetLoadingDevicesAsync(LoadingDevicesFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.LoadingDevices
            .Select(x => new LoadingDeviceResponse
            {
                Id = x.Id,
                DeviceKey = x.DeviceKey,
                Name = x.Name,
                LastActivityWorkerCodeWms = x.LastActivityWorkerCodeWms,
                BlockedFrom = x.BlockedFrom,
                UsedFrom = x.UsedFrom,
                LastActivity = x.LastActivity
            });

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}