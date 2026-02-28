using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Commands.DeleteKit;

internal record DeleteKitCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;