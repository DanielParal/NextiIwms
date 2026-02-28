using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.KitTypes.Commands.UpdateKitType;

internal record UpdateKitTypeCommand(string Code, string Name) : ISettingsCommand<ErrorOr<Updated>>;