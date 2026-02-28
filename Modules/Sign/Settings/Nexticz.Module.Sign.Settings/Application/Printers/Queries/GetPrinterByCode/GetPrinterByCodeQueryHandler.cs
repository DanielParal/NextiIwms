using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrinterByCode;

internal class GetPrinterByCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetPrinterByCodeQuery, ErrorOr<Printer>>
{
    public async Task<ErrorOr<Printer>> Handle(GetPrinterByCodeQuery request, CancellationToken cancellationToken)
    {
        var printer = await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<Printer>(
                x => x.Code.Equals(request.Code, StringComparison.InvariantCultureIgnoreCase), 
                cancellationToken);

        if (printer is null)
            return PrinterErrors.CodeDoesNotExist;
        
        return printer;
    }
}