namespace Nexticz.Module.Vh.Contracts.VhUsers;

public class UpdateVhUserRequest
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required bool Active { get; set; }
    public string[] Centers { get; set; } = [];
}