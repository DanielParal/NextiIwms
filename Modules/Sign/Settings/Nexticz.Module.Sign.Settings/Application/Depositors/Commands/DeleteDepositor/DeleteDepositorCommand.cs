using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Commands.DeleteDepositor;

internal record DeleteDepositorCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;