namespace Nexticz.Module.Auth.Contracts.Authentications;

public class GetAccountInfoResponse
{
    public required string Username { get; set; }
    public bool Registered { get; set; }
    public bool Blocked { get; set; }
    public string? BlockedReason { get; set; }
    public required List<string> AuthMethods { get; set; }
}