using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Commands.DeletePackagingType;

internal record DeletePackagingTypeCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;