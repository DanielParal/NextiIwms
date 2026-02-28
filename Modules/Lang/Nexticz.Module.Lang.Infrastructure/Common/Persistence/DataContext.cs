using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Lang.Application.Common.Helpers;
using Nexticz.Module.Lang.Domain.Languages;
using Nexticz.Module.Lang.Domain.Translations;

namespace Nexticz.Module.Lang.Infrastructure.Common.Persistence;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> option): base(option)
    {
    }

    public DbSet<Translation> Translations { get; set; } = null!;
    public DbSet<Language> Languages { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        builder.HasDefaultSchema(StringHelper.MigrationSchema);
    }
}