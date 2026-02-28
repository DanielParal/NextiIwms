using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Locations.Commands.UpdateLocation;

internal record UpdateLocationCommand(string Code, string Name) : ISettingsCommand<ErrorOr<Updated>>;