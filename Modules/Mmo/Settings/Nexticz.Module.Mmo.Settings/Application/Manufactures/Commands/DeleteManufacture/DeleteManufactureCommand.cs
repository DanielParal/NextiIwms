using MediatR;
using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Application.Manufactures.Commands.DeleteManufacture;

internal record DeleteManufactureCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;