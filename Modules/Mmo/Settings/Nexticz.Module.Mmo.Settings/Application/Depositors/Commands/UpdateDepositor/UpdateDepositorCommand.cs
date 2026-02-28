using MediatR;
using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Application.Depositors.Commands.UpdateDepositor;

internal record UpdateDepositorCommand(string Code, string Name, string? BarcodeTemplate) : ISettingsCommand<ErrorOr<Updated>>;