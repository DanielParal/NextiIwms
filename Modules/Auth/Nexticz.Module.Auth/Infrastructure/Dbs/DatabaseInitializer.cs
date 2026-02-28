using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Auth.Infrastructure.Dbs.Seeds;

namespace Nexticz.Module.Auth.Infrastructure.Dbs;

internal class DatabaseInitializer(
    AppRoleSeeder roleSeeder, 
    AppUserSeeder userSeeder,
    TranslationsSeeder translationsSeeder,
    AuthDataContext context,
    ILogger<DatabaseInitializer> logger) : IDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        try
        {
            await context.Database.MigrateAsync(cancellationToken);
            
            await roleSeeder.SeedAsync();
            await userSeeder.SeedAsync(); 
            await translationsSeeder.SeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Auth] [Error] [DatabaseInitializer] Database migration error. ErrorMessage: {ErrorMessage}",
                ex.Message);
        }
    }
}