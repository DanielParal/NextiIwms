using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.SigningDevices.Notifications;

public record SigningDeviceDeletedNotification(string Code) : INotification;