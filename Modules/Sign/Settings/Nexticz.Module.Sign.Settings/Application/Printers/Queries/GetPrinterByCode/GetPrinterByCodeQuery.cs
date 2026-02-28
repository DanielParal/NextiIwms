using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrinterByCode;

internal record GetPrinterByCodeQuery(string Code) : IRequest<ErrorOr<Printer>>; 