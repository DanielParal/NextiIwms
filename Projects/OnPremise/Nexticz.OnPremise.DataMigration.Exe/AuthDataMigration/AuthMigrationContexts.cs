
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Auth.Infrastructure.Dbs.TableConfigurations;
using Nexticz.OnPremise.DataMigration.Exe.AuthDataMigration.Identity;
using Nexticz.OnPremise.DataMigration.Exe.AuthDataMigration.Identity.Configurations;

namespace Nexticz.OnPremise.DataMigration.Exe.AuthDataMigration;

public class AuthMsSqlContext(DbContextOptions<AuthMsSqlContext> options)
    : IdentityDbContext<AppUser, AppRole, Guid, AppUserClaim, AppUserRole,
        IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>(options)
{

    public static readonly string MigrationTableName = "__MigrationHistory";

    public DbSet<AppUserRefreshToken> AppUserRefreshTokens { get; set; } = null!;
    public DbSet<AppUserApiKey> AppUserApiKeys { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.HasDefaultSchema("Auth");
    }
}

public class AuthPostgresContext(DbContextOptions<AuthPostgresContext> options) 
    : IdentityDbContext<AppUser, AppRole, Guid, AppUserClaim, AppUserRole,
    IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>(options)
{

    public static readonly string MigrationTableName = "__MigrationHistory";

    public DbSet<AppUserRefreshToken> AppUserRefreshTokens { get; set; } = null!;
    public DbSet<AppUserApiKey> AppUserApiKeys { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
    