using Nexticz.Module.Vh.Domain.Centers;

namespace Nexticz.Module.Vh.Domain.VhUsers;

public class VhUser
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required bool Active { get; set; }
    public ICollection<Center>? Centers { get; set; }
}