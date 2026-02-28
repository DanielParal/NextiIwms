using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.KitTypes.Commands.CreateKitType;

internal record CreateKitTypeCommand(string Code, string Name) : ISettingsCommand<ErrorOr<KitType>>;