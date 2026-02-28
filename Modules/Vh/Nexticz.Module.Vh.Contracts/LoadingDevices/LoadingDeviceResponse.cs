namespace Nexticz.Module.Vh.Contracts.LoadingDevices;

public class LoadingDeviceResponse
{
    public Guid Id { get; set; }
    public required Guid DeviceKey { get; set; }
    public string? Name { get; set; }
    public DateTime? BlockedFrom { get; set; }
    public DateTime? UsedFrom { get; set; }
    public DateTime? LastActivity { get; set; }
    public string? LastActivityWorkerCodeWms { get; set; }
}