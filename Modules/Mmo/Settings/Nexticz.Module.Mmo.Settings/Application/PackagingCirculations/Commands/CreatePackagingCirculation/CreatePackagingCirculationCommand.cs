using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Commands.CreatePackagingCirculation;

internal record CreatePackagingCirculationCommand(string Code, string Name) : ISettingsCommand<ErrorOr<PackagingCirculation>>;