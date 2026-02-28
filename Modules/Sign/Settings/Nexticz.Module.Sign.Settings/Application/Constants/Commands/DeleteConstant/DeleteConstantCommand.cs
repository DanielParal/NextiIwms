using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Constants.Commands.DeleteConstant;

internal record DeleteConstantCommand(string Key) : ISettingsCommand<ErrorOr<Deleted>>;