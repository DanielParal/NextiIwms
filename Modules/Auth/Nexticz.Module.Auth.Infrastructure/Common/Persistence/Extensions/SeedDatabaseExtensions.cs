using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Auth.Infrastructure.Common.Persistence.Initialization;

namespace Nexticz.Module.Auth.Infrastructure.Common.Persistence.Extensions;

public static class SeedDatabaseExtensions
{
    public static async Task SeedAuthDatabaseAsync(this WebApplication webApplication, WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var dataConntext = services.GetRequiredService<DataContext>();
        try
        {
            await dataConntext.Database.MigrateAsync();
            await new DatabaseInitializer(services, app.Configuration).RunSeed();
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning("Database migration or seed error {@Error}", ex.Message);
        }
    }
}