using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Commands.DeleteKitInstruction;

internal record DeleteKitInstructionCommand(string KitCode) : ISettingsCommand<ErrorOr<Success>>;