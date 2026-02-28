using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetSigningDeviceByCode;

internal class GetSigningDeviceByCodeQueryHandler(
    IDocumentManagerReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IRequestHandler<GetSigningDeviceByCodeQuery, ErrorOr<SigningDevice>>
{
    public async Task<ErrorOr<SigningDevice>> Handle(GetSigningDeviceByCodeQuery request, CancellationToken cancellationToken)
    {
        var upperCode = request.Code.ToUpperInvariant();
        var signingDevice = await readOnlyEventStoreRepository.GetFirstByConditionAsync<SigningDevice>(
            x => x.Code == upperCode, cancellationToken);
        
        if (signingDevice is null)
            return SigningDeviceErrors.SigningDeviceNotFound;
        
        return signingDevice;
    }
}