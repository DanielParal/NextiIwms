namespace Nexticz.Module.Vh.Contracts.Workers;

public class CreateWorkerRequest
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string CodeWms { get; set; }
    public string? CodeSag { get; set; }
    public string ActivityAfterCutOffCode { get; set; } = "";
    public required Guid CenterId { get; set; }
}