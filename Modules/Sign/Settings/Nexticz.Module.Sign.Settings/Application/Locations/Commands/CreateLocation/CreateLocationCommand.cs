using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Locations.Commands.CreateLocation;

internal record CreateLocationCommand(string Code, string Name) : ISettingsCommand<ErrorOr<Location>>;