using ErrorOr;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;

namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines.Commands.CreateWashingMachine;

internal record CreateWashingMachineCommand(CreateWashingMachineRequest CreateWashingMachineRequest) : ISettingsCommand<ErrorOr<WashingMachine>>;