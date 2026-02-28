using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Commands.DeleteKitSapDefinition;

internal record DeleteKitSapDefinitionCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;