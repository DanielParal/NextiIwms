using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.SigningDevices.Notifications;

public record SigningDeviceCreatedNotification(string Code, string Name, bool IsActive, string PrinterCode) : INotification;