using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Commands.UpdatePackagingType;

internal record UpdatePackagingTypeCommand(string Code, string Name) : ISettingsCommand<ErrorOr<Updated>>;