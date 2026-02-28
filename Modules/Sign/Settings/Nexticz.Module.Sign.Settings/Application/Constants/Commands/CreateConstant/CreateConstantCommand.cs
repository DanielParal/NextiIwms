using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.ConstantAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Constants.Commands.CreateConstant;

internal record CreateConstantCommand(
    string Key, string Value, ConstantType Type, string? Description) 
    : ISettingsCommand<ErrorOr<Constant>>;