using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Commands.UpdateKitSapDefinition;

internal record UpdateKitSapDefinitionCommand(string Code, string Name) : ISettingsCommand<ErrorOr<Updated>>;