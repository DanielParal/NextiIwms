using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Constants.Commands.CreateConstant;

internal record CreateConstantCommand(
    string Key, string Value, ConstantType Type, string? Description) 
    : ISettingsCommand<ErrorOr<Constant>>;