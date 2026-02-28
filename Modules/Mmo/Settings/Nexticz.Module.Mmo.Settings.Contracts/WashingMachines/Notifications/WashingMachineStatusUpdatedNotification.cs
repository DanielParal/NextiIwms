using MediatR;

namespace Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Notifications;

public record WashingMachineStatusUpdatedNotification(
string WashingMachineCode, WashingMachineStatusContract Status, WashingMachineLineContract[] WashingMachineLines) : INotification;