using ErrorOr;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Commands.UpdateWashingMachineSpeed;

internal record UpdateWashingMachineSpeedCommand(Guid Id, string Code, int Speed, SpeedLevel SpeedLevel) : IWashingCommand<ErrorOr<Success>>;