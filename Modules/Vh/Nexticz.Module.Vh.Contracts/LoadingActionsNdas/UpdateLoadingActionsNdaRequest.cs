namespace Nexticz.Module.Vh.Contracts.LoadingActionsNdas;

public class UpdateLoadingActionsNdaRequest
{
    public required DateTime Created { get; set; }
    public string? Note { get; set; }
    public required Guid LoadingDeviceId { get; set; }
    public required string WorkerSlug { get; set; }
    public required string NonDispensingActivitySlug { get; set; }
}