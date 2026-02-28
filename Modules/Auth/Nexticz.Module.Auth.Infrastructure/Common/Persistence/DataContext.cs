using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Auth.Application.Common.Helpers;
using Nexticz.Module.Auth.Domain.AppRoles;
using Nexticz.Module.Auth.Domain.AppUserApiKeys;
using Nexticz.Module.Auth.Domain.AppUserClaims;
using Nexticz.Module.Auth.Domain.AppUserRefreshTokens;
using Nexticz.Module.Auth.Domain.AppUserRoles;
using Nexticz.Module.Auth.Domain.AppUsers;

namespace Nexticz.Module.Auth.Infrastructure.Common.Persistence;

public class DataContext : IdentityDbContext<AppUser, AppRole, Guid, AppUserClaim, AppUserRole, 
    IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public DataContext(DbContextOptions<DataContext> option): base(option)
    {
    }
    
    public DbSet<AppUserRefreshToken> AppUserRefreshTokens { get; set; } = null!;
    public DbSet<AppUserApiKey> AppUserApiKeys { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        builder.HasDefaultSchema(StringHelper.MigrationSchema);
    }
}