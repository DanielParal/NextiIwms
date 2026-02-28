using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;

namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines.Commands.UpdateWashingMachine;

internal record UpdateWashingMachineCommand(string Code, UpdateWashingMachineRequest UpdateWashingMachineRequest) : ISettingsCommand<ErrorOr<Updated>>;