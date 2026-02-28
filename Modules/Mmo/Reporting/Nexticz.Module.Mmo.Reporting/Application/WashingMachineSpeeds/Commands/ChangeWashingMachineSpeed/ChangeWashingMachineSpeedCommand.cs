using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.WashingMachineSpeeds.Commands.ChangeWashingMachineSpeed;

internal record ChangeWashingMachineSpeedCommand(
    string Code, int Speed, SpeedLevel SpeedLevel, DateTimeOffset DateChanged) : IReportingCommand<ErrorOr<Success>>;