using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.Manufactures.Commands.UpdateManufacture;

internal record UpdateManufactureCommand(string Code, string Name) : ISettingsCommand<ErrorOr<Updated>>;