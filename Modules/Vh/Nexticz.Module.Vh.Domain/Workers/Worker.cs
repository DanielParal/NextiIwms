using Nexticz.Module.Vh.Domain.Centers;
using Nexticz.Module.Vh.Domain.LoadingActionsNdas;

namespace Nexticz.Module.Vh.Domain.Workers;

public class Worker
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string CodeWms { get; set; }
    public string? CodeSag { get; set; }
    public required Guid CenterId { get; set; }
    public Center? Center { get; set; }
    public string? ActivityAfterCutOffCode { get; set; }
    public ICollection<LoadingActionsNda>? LoadingActionsNdas { get; set; }
}