using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Commands.CreatePrinter;

internal record CreatePrinterCommand(string Code, string Name, string Ip) : ISettingsCommand<ErrorOr<Printer>>;