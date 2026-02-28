using Nexticz.Module.Sign.Settings.Contracts.Printers;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.Printers;

internal static class PrinterResponseFactory
{
    public static PrinterResponse Create(Printer printer)
    {
        return new PrinterResponse(printer.Id, printer.Code, printer.Name, printer.Ip);
    }
}