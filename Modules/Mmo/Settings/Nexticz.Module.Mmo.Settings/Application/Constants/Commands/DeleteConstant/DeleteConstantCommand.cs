using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.Constants.Commands.DeleteConstant;

internal record DeleteConstantCommand(string Key) : ISettingsCommand<ErrorOr<Deleted>>;