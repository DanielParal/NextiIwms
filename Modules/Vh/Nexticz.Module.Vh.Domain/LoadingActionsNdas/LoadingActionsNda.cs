using Nexticz.Module.Vh.Domain.LoadingDevices;
using Nexticz.Module.Vh.Domain.NonDispensingActivities;
using Nexticz.Module.Vh.Domain.Workers;

namespace Nexticz.Module.Vh.Domain.LoadingActionsNdas;

public class LoadingActionsNda
{
    public Guid Id { get; set; }
    public required DateTime Created { get; set; }
    public string? Note { get; set; }
    public required Guid LoadingDeviceId { get; set; }
    public LoadingDevice? LoadingDevice { get; set; }
    public required string WorkerSlug { get; set; }
    public Worker? Worker { get; set; }
    public required string NonDispensingActivitySlug { get; set; }
    public NonDispensingActivity? NonDispensingActivity { get; set; }
}