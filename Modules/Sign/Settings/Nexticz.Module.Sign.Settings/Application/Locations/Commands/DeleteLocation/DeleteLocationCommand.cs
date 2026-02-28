using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Locations.Commands.DeleteLocation;

internal record DeleteLocationCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;