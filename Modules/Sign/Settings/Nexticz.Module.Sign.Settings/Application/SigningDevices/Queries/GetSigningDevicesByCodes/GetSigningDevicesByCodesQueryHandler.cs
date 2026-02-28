using Marten;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDevicesByCodes;

internal class GetSigningDevicesByCodesQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetSigningDevicesByCodesQuery, SigningDevice[]>
{
    public async Task<SigningDevice[]> Handle(GetSigningDevicesByCodesQuery request, CancellationToken cancellationToken)
    {
        var upperCodes = request.Codes.Select(x => x.ToUpperInvariant()).ToArray();
        var signingDevices = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<SigningDevice>(
                x => x.Code.IsOneOf(upperCodes), 
                cancellationToken);

        return signingDevices.ToArray();
    }
}