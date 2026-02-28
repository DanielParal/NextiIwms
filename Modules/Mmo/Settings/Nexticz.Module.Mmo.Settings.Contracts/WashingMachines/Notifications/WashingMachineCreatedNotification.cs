using MediatR;

namespace Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Notifications;

public record WashingMachineCreatedNotification(
    string WashingMachineCode, WashingMachineStatusContract Status, WashingMachineLineContract[] WashingMachineLines) : INotification;