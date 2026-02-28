using MediatR;
using Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDevicesByCodes;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDevicesByPrinterCode;

internal class GetSigningDevicesByPrinterCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetSigningDevicesByPrinterCodeQuery, SigningDevice[]>
{
    public async Task<SigningDevice[]> Handle(GetSigningDevicesByPrinterCodeQuery request, CancellationToken cancellationToken)
    {
        var upperPrinterCode = request.PrinterCode.ToUpperInvariant();
        var signingDevices = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<SigningDevice>(
                x => x.PrinterCode.Equals(upperPrinterCode), 
                cancellationToken);

        return signingDevices.ToArray();
    }
}