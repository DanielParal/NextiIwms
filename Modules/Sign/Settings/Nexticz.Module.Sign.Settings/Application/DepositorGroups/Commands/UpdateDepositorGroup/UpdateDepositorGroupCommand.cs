using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.Commands.UpdateDepositorGroup;

internal record UpdateDepositorGroupCommand(string Code, string Name) : ISettingsCommand<ErrorOr<Updated>>;