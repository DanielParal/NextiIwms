using MediatR;
using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.Printers.Queries.GetPrinterByCode;

internal record GetPrinterByCodeQuery(string Code) : IRequest<ErrorOr<Printer>>;