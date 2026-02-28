using ErrorOr;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Commands.CreateWashingMachineSpeed;

internal record CreateWashingMachineSpeedCommand(string Code, int Speed, SpeedLevel SpeedLevel) : IWashingCommand<ErrorOr<Success>>;