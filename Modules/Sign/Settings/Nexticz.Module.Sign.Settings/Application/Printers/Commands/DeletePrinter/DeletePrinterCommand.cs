using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Commands.DeletePrinter;

internal record DeletePrinterCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;