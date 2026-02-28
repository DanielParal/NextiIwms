using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Commands.UpdatePrinter;

internal record UpdatePrinterCommand(string Code, string Name, string Ip) : ISettingsCommand<ErrorOr<Updated>>;