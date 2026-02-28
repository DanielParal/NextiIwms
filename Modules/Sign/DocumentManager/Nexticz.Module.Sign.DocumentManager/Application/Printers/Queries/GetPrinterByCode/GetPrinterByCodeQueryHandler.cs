using MediatR;
using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.Printers.Queries.GetPrinterByCode;

internal class GetPrinterByCodeQueryHandler(
    IDocumentManagerReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IRequestHandler<GetPrinterByCodeQuery, ErrorOr<Printer>>
{
    public async Task<ErrorOr<Printer>> Handle(GetPrinterByCodeQuery request, CancellationToken cancellationToken)
    {
        var upperCode = request.Code.ToUpperInvariant();
        var printer = await readOnlyEventStoreRepository.GetFirstByConditionAsync<Printer>(
            x => x.Code == upperCode, cancellationToken);
        
        if (printer is null)
            return PrinterErrors.PrinterNotFound;
        
        return printer;
    }
}