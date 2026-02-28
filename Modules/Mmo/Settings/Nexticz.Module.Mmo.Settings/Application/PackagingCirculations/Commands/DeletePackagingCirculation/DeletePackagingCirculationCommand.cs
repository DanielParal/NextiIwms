using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Commands.DeletePackagingCirculation;

internal record DeletePackagingCirculationCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;