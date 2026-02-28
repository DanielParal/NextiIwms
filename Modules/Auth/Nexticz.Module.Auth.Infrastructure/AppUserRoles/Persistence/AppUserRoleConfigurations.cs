using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Auth.Domain.AppUserRoles;

namespace Nexticz.Module.Auth.Infrastructure.AppUserRoles.Persistence;

public class AppUserRoleConfigurations : IEntityTypeConfiguration<AppUserRole>
{
    public void Configure(EntityTypeBuilder<AppUserRole> builder)
    {
        builder.ToTable(name: "AppUserRoles");
    }
}