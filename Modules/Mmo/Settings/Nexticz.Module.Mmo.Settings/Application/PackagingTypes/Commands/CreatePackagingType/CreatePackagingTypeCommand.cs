using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Commands.CreatePackagingType;

internal record CreatePackagingTypeCommand(string Code, string Name) : ISettingsCommand<ErrorOr<PackagingType>>;