using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.SigningDevices.Notifications;

public record SigningDeviceUpdatedNotification(string Code, string Name, bool IsActive, string PrinterCode) : INotification;