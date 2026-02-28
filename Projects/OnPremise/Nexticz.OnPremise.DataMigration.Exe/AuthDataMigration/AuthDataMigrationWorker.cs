using System.Diagnostics;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace Nexticz.OnPremise.DataMigration.Exe.AuthDataMigration;

internal class AuthDataMigrationWorker(
    IFeatureManager featureManager,
    IServiceProvider serviceProvider,
    ILogger<AuthDataMigrationWorker> logger)
    : BackgroundService
{
    private const string FeatureFlagName = "EnableAuthDataMigration";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await featureManager.IsEnabledAsync(FeatureFlagName))
        {
            logger.LogInformation("Migration is disabled (Feature flag '{FeatureFlag}' is off).", FeatureFlagName);
            return;
        }

        logger.LogInformation("Starting Auth Data Migration...");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var scope = serviceProvider.CreateScope();
            var sqlContext = scope.ServiceProvider.GetRequiredService<AuthMsSqlContext>();
            var pgContext = scope.ServiceProvider.GetRequiredService<AuthPostgresContext>();

            // 1. Roles
            await MigrateTableAsync(
                sqlContext.Roles,
                pgContext.Roles,
                pgContext,
                r => r.Id,
                "Roles",
                stoppingToken);

            // 2. Users
            await MigrateTableAsync(
                sqlContext.Users,
                pgContext.Users,
                pgContext,
                u => u.Id,
                "Users",
                stoppingToken);

            // 3. UserRoles
            await MigrateTableAsync(
                sqlContext.UserRoles,
                pgContext.UserRoles,
                pgContext,
                ur => new { ur.UserId, ur.RoleId },
                "UserRoles",
                stoppingToken);

            // 4. UserClaims
            await MigrateTableAsync(
                sqlContext.UserClaims,
                pgContext.UserClaims,
                pgContext,
                uc => uc.Id,
                "UserClaims",
                stoppingToken);

            // 5. UserLogins
            await MigrateTableAsync(
                sqlContext.UserLogins,
                pgContext.UserLogins,
                pgContext,
                ul => new { ul.LoginProvider, ul.ProviderKey },
                "UserLogins",
                stoppingToken);

            // 6. UserTokens
            await MigrateTableAsync(
                sqlContext.UserTokens,
                pgContext.UserTokens,
                pgContext,
                ut => new { ut.UserId, ut.LoginProvider, ut.Name },
                "UserTokens",
                stoppingToken);

            // 7. RoleClaims
            await MigrateTableAsync(
                sqlContext.RoleClaims,
                pgContext.RoleClaims,
                pgContext,
                rc => rc.Id,
                "RoleClaims",
                stoppingToken);

            // 8. RefreshTokens
            await MigrateTableAsync(
                sqlContext.AppUserRefreshTokens,
                pgContext.AppUserRefreshTokens,
                pgContext,
                rt => rt.Id,
                "RefreshTokens",
                stoppingToken);

            // 9. ApiKeys
            await MigrateTableAsync(
                sqlContext.AppUserApiKeys,
                pgContext.AppUserApiKeys,
                pgContext,
                ak => ak.Id,
                "ApiKeys",
                stoppingToken);

            stopwatch.Stop();
            logger.LogInformation("Auth Data Migration completed successfully in {Duration}.", stopwatch.Elapsed);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Auth Data Migration was canceled.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during Auth Data Migration.");
        }
    }

    private async Task MigrateTableAsync<TEntity, TKey>(
        IQueryable<TEntity> sourceSet,
        DbSet<TEntity> targetSet,
        DbContext targetContext,
        Func<TEntity, TKey> keySelector,
        string tableName,
        CancellationToken stoppingToken)
        where TEntity : class
    {
        logger.LogInformation("Migrating table '{TableName}'...", tableName);
        var sw = Stopwatch.StartNew();

        var sourceData = await sourceSet.AsNoTracking().ToListAsync(stoppingToken);
        int readCount = sourceData.Count;
        int insertedCount = 0;
        int updatedCount = 0;

        foreach (var entity in sourceData)
        {
            if (stoppingToken.IsCancellationRequested) break;

            NormalizeDateTimesToUtc(entity);
            
            var key = keySelector(entity);
            var existing = await FindExistingAsync(targetSet, key, stoppingToken);

            if (existing == null)
            {
                targetSet.Add(entity);
                insertedCount++;
            }
            else
            {
                targetContext.Entry(existing).CurrentValues.SetValues(entity);
                updatedCount++;
            }
        }

        if (insertedCount > 0 || updatedCount > 0)
        {
            await targetContext.SaveChangesAsync(stoppingToken);
        }

        sw.Stop();
        logger.LogInformation(
            "Finished '{TableName}': Read: {Read}, Inserted: {Inserted}, Updated: {Updated}. Duration: {Duration}",
            tableName, readCount, insertedCount, updatedCount, sw.Elapsed);
    }
    
    private static void NormalizeDateTimesToUtc<TEntity>(TEntity entity)
        where TEntity : class
    {
        var props = entity.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.CanRead && p.CanWrite);

        foreach (var p in props)
        {
            if (p.PropertyType == typeof(DateTime))
            {
                var dt = (DateTime)p.GetValue(entity)!;
                p.SetValue(entity, Normalize(dt));
                continue;
            }

            if (p.PropertyType == typeof(DateTime?))
            {
                var dt = (DateTime?)p.GetValue(entity);
                if (dt is not null)
                {
                    p.SetValue(entity, Normalize(dt.Value));
                }
            }
        }

        static DateTime Normalize(DateTime dt)
        {
            return dt.Kind switch
            {
                DateTimeKind.Utc => dt,
                DateTimeKind.Local => dt.ToUniversalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
                _ => DateTime.SpecifyKind(dt, DateTimeKind.Utc)
            };
        }
    }

    private async Task<TEntity?> FindExistingAsync<TEntity, TKey>(
        DbSet<TEntity> targetSet,
        TKey key,
        CancellationToken stoppingToken)
        where TEntity : class
    {
        if (key is Guid guidKey)
        {
            return await targetSet.FindAsync([guidKey], stoppingToken);
        }
        
        if (key is int intKey)
        {
             return await targetSet.FindAsync([intKey], stoppingToken);
        }

        // Handle composite keys or other types if necessary. 
        // FindAsync accepts params object[] keyValues.
        if (key is { } and not string)
        {
            // For anonymous types used in keySelector (e.g., UserRoles, UserLogins, UserTokens)
            var properties = key.GetType().GetProperties();
            var values = properties.Select(p => p.GetValue(key)).ToArray();
            return await targetSet.FindAsync(values, stoppingToken);
        }

        return await targetSet.FindAsync([key], stoppingToken);
    }
}