using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.KitTypes.Commands.DeleteKitType;

internal record DeleteKitTypeCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;