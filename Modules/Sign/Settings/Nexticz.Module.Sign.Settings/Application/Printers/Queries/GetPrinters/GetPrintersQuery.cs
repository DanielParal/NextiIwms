using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrinters;

internal record GetPrintersQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Printer>>;