using Marten;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrintersByCodes;

internal class GetPrintersByCodesQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)
    : IRequestHandler<GetPrintersByCodesQuery, Printer[]>
{
    public async Task<Printer[]> Handle(GetPrintersByCodesQuery request, CancellationToken cancellationToken)
    {
        var upperCodes = request.Codes.Select(x => x.ToUpperInvariant()).ToArray();
        var printers = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Printer>(
                x => x.Code.IsOneOf(upperCodes), 
                cancellationToken);

        return printers.ToArray();
    }
}