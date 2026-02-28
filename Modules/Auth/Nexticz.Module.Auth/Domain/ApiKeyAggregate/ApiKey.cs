using System.Security.Cryptography;
using Nexticz.Lib.Shared.DomainCore;
using ErrorOr;

namespace Nexticz.Module.Auth.Domain.ApiKeyAggregate;

public class ApiKey : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string Description { get; private set; }
    public string Value { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public DateTimeOffset? LastActivity { get; private set; }
    public DateTimeOffset? Expiration { get; private set; }

    private ApiKey(
        Guid userId,
        string description,
        string value,
        DateTimeOffset created,
        DateTimeOffset? lastActivity = null,
        DateTimeOffset? expiration = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        UserId = userId;
        Description = description;
        Value = value;
        Created = created;
        LastActivity = lastActivity;
        Expiration = expiration;
    }
    
    public static ErrorOr<ApiKey> CreateFrom(
        Guid userId,
        string description,
        DateTimeOffset created,
        DateTimeOffset? lastActivity = null,
        DateTimeOffset? expiration = null,
        Guid? id = null)
    {
        var value = GenerateApiKey();
        return new ApiKey(userId, description, value, created, lastActivity, expiration, id);
    }
    
    public static ErrorOr<ApiKey> CreateFrom(
        Guid id,
        Guid userId,
        string value,
        string description,
        DateTimeOffset created,
        DateTimeOffset? lastActivity = null,
        DateTimeOffset? expiration = null)
    {
        return new ApiKey(userId, description, value, created, lastActivity, expiration, id);
    }
    
    private static string GenerateApiKey()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}