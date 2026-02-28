using MediatR;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrintersByCodes;

internal record GetPrintersByCodesQuery(string[] Codes) : IRequest<Printer[]>;