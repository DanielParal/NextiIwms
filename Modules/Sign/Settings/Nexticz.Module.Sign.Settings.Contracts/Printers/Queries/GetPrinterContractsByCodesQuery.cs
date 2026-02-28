using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.Printers.Queries;

public record GetPrinterContractsByCodesQuery(string[] Codes) : IRequest<PrinterContract[]>;