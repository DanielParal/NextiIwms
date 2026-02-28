using Nexticz.Module.Sign.Settings.Contracts.Printers;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Printers;

internal static class PrinterContractFactory
{
    public static PrinterContract Create(Printer printer)
    {
        return new PrinterContract(printer.Code, printer.Ip);
    }
}