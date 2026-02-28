using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Commands.UpdateSpecialInformation;

internal record UpdateSpecialInformationCommand(Guid Id, string Title, string Description) : ISettingsCommand<ErrorOr<Updated>>;