using MediatR;
using ErrorOr;
using Nexticz.Module.Sign.Settings.Contracts.Printers;
using Nexticz.Module.Sign.Settings.Contracts.Printers.Queries;
using Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrinterByCode;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrinterContractByCode;

internal class GetPrinterContractByCodeRequestHandler(ISender sender) : IRequestHandler<GetPrinterContractByCodeRequest, ErrorOr<PrinterContract>>
{
    public async Task<ErrorOr<PrinterContract>> Handle(GetPrinterContractByCodeRequest request, CancellationToken cancellationToken)
    {
        var printer = await sender.Send(new GetPrinterByCodeQuery(request.Code), cancellationToken);

        if (printer.IsError)
            return printer.Errors;

        return PrinterContractFactory.Create(printer.Value);
    }
}