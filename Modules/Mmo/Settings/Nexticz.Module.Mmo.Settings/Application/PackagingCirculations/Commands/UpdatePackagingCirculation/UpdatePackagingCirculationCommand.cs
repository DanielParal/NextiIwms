using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Commands.UpdatePackagingCirculation;

internal record UpdatePackagingCirculationCommand(string Code, string Name) : ISettingsCommand<ErrorOr<Updated>>;