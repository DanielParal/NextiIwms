using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Constants.Commands.UpdateConstant;

internal record UpdateConstantCommand(string Key, string Value, string? Description) : ISettingsCommand<ErrorOr<Updated>>;