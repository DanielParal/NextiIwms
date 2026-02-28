using Nexticz.Module.Vh.Domain.LoadingActionsNdas;

namespace Nexticz.Module.Vh.Domain.LoadingDevices;

public class LoadingDevice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeviceKey { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public DateTime? BlockedFrom { get; set; }
    public DateTime? UsedFrom { get; set; }
    public DateTime? LastActivity { get; set; }
    public string? LastActivityWorkerCodeWms { get; set; }
    public ICollection<LoadingActionsNda>? LoadingActionsNdas { get; set; }
}