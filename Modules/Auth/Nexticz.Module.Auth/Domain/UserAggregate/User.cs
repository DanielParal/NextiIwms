using ErrorOr;
using Nexticz.Lib.Shared.DomainCore;
using Nexticz.Lib.Shared.Emails;

namespace Nexticz.Module.Auth.Domain.UserAggregate;

public class User : AggregateRoot
{
    public string UserName { get; private set; }
    public string Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? Company { get; private set; }
    public DateTime? LastActivity { get; private set; }
    public DateTime? BlockedFrom { get; private set; }
    public string[] Roles { get; private set; }
    public string[] Permissions { get; private set; }
    public Guid[] ApiKeys { get; private set; }

    private User(
        string userName, 
        string email,
        string? phoneNumber, 
        string? firstName, 
        string? lastName, 
        string? company,
        string[] roles, 
        string[] permissions,
        Guid[] apiKeys,
        DateTime? blockedFrom = null,
        DateTime? lastActivity = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        UserName = userName;
        Email = email;
        PhoneNumber = phoneNumber;
        FirstName = firstName;
        LastName = lastName;
        Company = company;
        Roles = roles;
        Permissions = permissions;
        ApiKeys = apiKeys;
        BlockedFrom = blockedFrom;
        LastActivity = lastActivity;
    }
    
    public static ErrorOr<User> CreateFrom(
        string userName, 
        string email,
        string? phoneNumber, 
        string? firstName, 
        string? lastName, 
        string? company,
        string[] roles, 
        string[] permissions,
        Guid[] apiKeys,
        DateTime? blockedFrom = null,
        DateTime? lastActivity = null,
        Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return UserDomainErrors.ValidationUserNameMustBeFilledIn;
        
        if (string.IsNullOrWhiteSpace(email))
            return UserDomainErrors.ValidationEmailMustBeFilledIn;

        if (!EmailValidator.IsValidEmail(email))
            return UserDomainErrors.ValidationInvalidEmailFormat;
        
        return new User(
            userName, email, phoneNumber, firstName, lastName, company, 
            roles, permissions, apiKeys, blockedFrom, lastActivity, id);
    }

    public ErrorOr<Success> Update(
        string? email, 
        string? phoneNumber, 
        string? firstName, 
        string? lastName, 
        string? company,
        DateTime? blockedFrom,
        string[] roles, 
        string[] permissions)
    {
        if (string.IsNullOrWhiteSpace(email))
            return UserDomainErrors.ValidationEmailMustBeFilledIn;
        
        if (!EmailValidator.IsValidEmail(email))
            return UserDomainErrors.ValidationInvalidEmailFormat;
        
        Email = email;
        PhoneNumber = phoneNumber;
        FirstName = firstName;
        LastName = lastName;
        Company = company;
        BlockedFrom = blockedFrom;
        
        Roles = roles.Length == 0 ? [nameof(RoleType.Anonymous)] : roles;
        Permissions = permissions.Length == 0 ? [nameof(PermissionType.Anonymous)] : permissions;
        
        return Result.Success;
    }
    
    public void AddDefaultRole()
    {
        Roles = Roles.Append(nameof(RoleType.Anonymous)).ToArray();
    }
    
    public void AddDefaultPermission()
    {
        Permissions = Permissions.Append(nameof(PermissionType.Anonymous)).ToArray();
    }
}