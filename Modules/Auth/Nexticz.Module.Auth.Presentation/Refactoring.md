

# Domain objects

Account = AppUser
ApiKey = AppUserApiKey
RefreshToken = AppUserRefreshToken

AppRoles and AppClaims are not used. We use only AppUserRoles and AppUserClaims where do we save 

AppRoles is filled in, but it is not maintained when something changes or deletes

AppUserLogins is not used

# Auth functionality:

Login, Logout, RefreshToken, Register, ChangePassword
Forgot password, Reset password


We need to create our own MartenUserStore:

```
public class MartenUserStore : 
    IUserStore<AppUser>,
    IUserPasswordStore<AppUser>,
    IUserLockoutStore<AppUser>,
    IUserEmailStore<AppUser>
{
    private readonly IDocumentSession _session;

    public MartenUserStore(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<IdentityResult> CreateAsync(AppUser user, CancellationToken ct)
    {
        _session.Store(user);
        await _session.SaveChangesAsync(ct);
        return IdentityResult.Success;
    }

    public Task<AppUser> FindByIdAsync(string id, CancellationToken ct)
        => _session.LoadAsync<AppUser>(id, ct);

    public Task<AppUser> FindByEmailAsync(string normalizedEmail, CancellationToken ct)
        => _session.Query<AppUser>().FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, ct);

    // Implement AccessFailedCount, LockoutEnd, PasswordHash, etc
}

```
# Common pattern

Leave AppUser in nosql database for SignInManager and use marten and events for the rest

https://chatgpt.com/share/693a82ba-869c-8001-b80f-b14462f118d4

Id, AggregateId, Data (json), EventType, TimeStamp, Headers (json)

# Marten Identity store

https://github.com/yetanotherchris/Marten.AspNetIdentity
https://github.com/Renzs90/Marten.Identity
https://www.nuget.org/packages/Marten.AspNetIdentity/8.0.12
https://github.com/OlegGavrilov/AspNetCore.Identity.Marten.Updated