using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDeviceByCode;

internal class GetSigningDeviceByCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetSigningDeviceByCodeQuery, ErrorOr<SigningDevice>>
{
    public async Task<ErrorOr<SigningDevice>> Handle(GetSigningDeviceByCodeQuery request, CancellationToken cancellationToken)
    {
        var signingDevice = await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<SigningDevice>(
                x => x.Code.Equals(request.Code, StringComparison.InvariantCultureIgnoreCase), 
                cancellationToken);

        if (signingDevice is null)
            return SigningDeviceErrors.CodeDoesNotExist;
        
        return signingDevice;
    }
}