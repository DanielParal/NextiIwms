using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDevicesByLocationCode;

internal class GetSigningDevicesByLocationCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetSigningDevicesByLocationCodeQuery, SigningDevice[]>
{
    public async Task<SigningDevice[]> Handle(GetSigningDevicesByLocationCodeQuery request, CancellationToken cancellationToken)
    {
        var upperLocationCode = request.LocationCode.ToUpperInvariant();
        var signingDevices = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<SigningDevice>(
                x => x.LocationCode.Equals(upperLocationCode), 
                cancellationToken);

        return signingDevices.ToArray();
    }
}