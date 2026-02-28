using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrinters;

internal class GetPrintersQueryHandler
    (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetPrintersQuery, FilteredResult<Printer>>
{
    public async Task<FilteredResult<Printer>> Handle(GetPrintersQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<Printer>(request.FilteringParams, cancellationToken);
    }
}