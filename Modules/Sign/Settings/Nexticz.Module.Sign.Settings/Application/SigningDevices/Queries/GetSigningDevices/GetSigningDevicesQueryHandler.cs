using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDevices;

internal class GetSigningDevicesQueryHandler
    (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetSigningDevicesQuery, FilteredResult<SigningDevice>>
{
    public async Task<FilteredResult<SigningDevice>> Handle(GetSigningDevicesQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<SigningDevice>(request.FilteringParams, cancellationToken);
    }
}