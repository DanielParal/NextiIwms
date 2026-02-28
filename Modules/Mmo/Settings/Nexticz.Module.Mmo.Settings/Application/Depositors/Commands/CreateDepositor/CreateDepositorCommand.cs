using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Depositors.Commands.CreateDepositor;

internal record CreateDepositorCommand(string Code, string Name, string? BarcodeTemplate) : ISettingsCommand<ErrorOr<Depositor>>;