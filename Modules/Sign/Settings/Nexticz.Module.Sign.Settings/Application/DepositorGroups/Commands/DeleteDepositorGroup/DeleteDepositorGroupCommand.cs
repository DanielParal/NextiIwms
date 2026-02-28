using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.Commands.DeleteDepositorGroup;

internal record DeleteDepositorGroupCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;