using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Auth.Domain.AppUserClaims;

namespace Nexticz.Module.Auth.Infrastructure.AppUserClaims.Persistence;

public class AppUserClaimConfigurations : IEntityTypeConfiguration<AppUserClaim>
{
    public void Configure(EntityTypeBuilder<AppUserClaim> builder)
    {
        builder.ToTable(name: "AppUserClaims");
    }
}