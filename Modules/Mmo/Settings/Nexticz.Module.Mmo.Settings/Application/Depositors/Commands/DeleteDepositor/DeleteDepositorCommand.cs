using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.Depositors.Commands.DeleteDepositor;

internal record DeleteDepositorCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;