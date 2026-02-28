namespace Nexticz.Module.Vh.Contracts.Workers;

public class UpdateWorkerRequest
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string CodeSag { get; set; }
    public string ActivityAfterCutOffCode { get; set; } = "";
    public required Guid CenterId { get; set; }
}