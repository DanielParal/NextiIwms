using ErrorOr;
using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.Printers.Queries;

public record GetPrinterContractByCodeRequest(string Code) : IRequest<ErrorOr<PrinterContract>>;
