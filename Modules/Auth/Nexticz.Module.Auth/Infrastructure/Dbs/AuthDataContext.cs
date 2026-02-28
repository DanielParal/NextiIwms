using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Auth.Infrastructure.Dbs.Tables;
using Nexticz.Module.Auth.Infrastructure.Identity;

namespace Nexticz.Module.Auth.Infrastructure.Dbs;

public class AuthDataContext(DbContextOptions<AuthDataContext> option)
    : IdentityDbContext<AppUser, AppRole, Guid, AppUserClaim, AppUserRole,
        IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>(option)
{
    
    public static readonly string MigrationTableName = "__MigrationHistory";
    
    public DbSet<AppUserRefreshToken> AppUserRefreshTokens { get; set; } = null!;
    public DbSet<AppUserApiKey> AppUserApiKeys { get; set; } = null!;
    public DbSet<EventLog> EventLogs { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
    }
}