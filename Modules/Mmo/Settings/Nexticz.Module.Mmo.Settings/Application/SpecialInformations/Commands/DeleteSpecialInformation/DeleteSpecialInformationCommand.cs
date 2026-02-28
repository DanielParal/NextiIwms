using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Commands.DeleteSpecialInformation;

internal record DeleteSpecialInformationCommand(Guid Id) : ISettingsCommand<ErrorOr<Deleted>>;