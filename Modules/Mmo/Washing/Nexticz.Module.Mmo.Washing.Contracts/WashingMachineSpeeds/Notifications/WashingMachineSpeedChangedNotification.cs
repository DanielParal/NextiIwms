using MediatR;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;

namespace Nexticz.Module.Mmo.Washing.Contracts.WashingMachineSpeeds.Notifications;

public record WashingMachineSpeedChangedNotification(
    string Code, int Speed, SpeedLevelContract SpeedLevel, DateTimeOffset DateChanged) : INotification;