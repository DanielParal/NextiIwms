using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Commands.DeleteSpecialInformationFile;

internal record DeleteSpecialInformationFileCommand(Guid Id) : ISettingsCommand<ErrorOr<Success>>;