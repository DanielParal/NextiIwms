using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.Constants.Commands.UpdateConstant;

internal record UpdateConstantCommand(string Key, string Value, string? Description) : ISettingsCommand<ErrorOr<Updated>>;