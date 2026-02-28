using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.Printers;
using Nexticz.Module.Sign.Settings.Contracts.Printers.Queries;
using Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrintersByCodes;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrinterContractsByCodes;

internal class GetPrinterContractsByCodesQueryHandler(ISender sender) : IRequestHandler<GetPrinterContractsByCodesQuery, PrinterContract[]>
{
    public async Task<PrinterContract[]> Handle(GetPrinterContractsByCodesQuery request, CancellationToken cancellationToken)
    {
        var printers = await sender.Send(new GetPrintersByCodesQuery(request.Codes), cancellationToken);
        return printers.Select(PrinterContractFactory.Create).ToArray();
    }
}