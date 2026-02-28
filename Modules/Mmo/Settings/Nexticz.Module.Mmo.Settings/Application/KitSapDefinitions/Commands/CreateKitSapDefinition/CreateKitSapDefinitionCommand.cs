using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;

namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Commands.CreateKitSapDefinition;

internal record CreateKitSapDefinitionCommand(string Code, string Name) : ISettingsCommand<ErrorOr<KitSapDefinition>>;