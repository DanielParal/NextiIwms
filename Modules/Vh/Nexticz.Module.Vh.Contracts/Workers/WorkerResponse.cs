using Nexticz.Module.Vh.Contracts.Centers;

namespace Nexticz.Module.Vh.Contracts.Workers;

public class WorkerResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string CodeWms { get; set; }
    public required string CodeSag { get; set; }
    public required string ActivityAfterCutOffCode { get; set; }
    public required Guid CenterId { get; set; }
    public CenterResponse? Center { get; set; }
}