using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Manufactures.Commands.CreateManufacture;

internal record CreateManufactureCommand(string Code, string Name) : ISettingsCommand<ErrorOr<Manufacture>>;