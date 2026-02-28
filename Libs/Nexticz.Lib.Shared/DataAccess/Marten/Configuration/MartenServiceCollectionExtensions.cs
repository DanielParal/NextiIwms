using DevExtreme.AspNet.Data.Async;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events.Daemon;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Lib.Shared.Testing;
using Weasel.Core;

namespace Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

public static class MartenServiceCollectionExtensions
{
    public static IServiceCollection AddMarten<T>(
        this IServiceCollection services, string schemaName, string connectionString, IConfiguration configuration) 
        where T : class, IDocumentStore
    {
        services.AddMartenStore<T>(options =>
            {
                options.DatabaseSchemaName = schemaName;
                options.Events.DatabaseSchemaName = schemaName;
                options.Events.MetadataConfig.HeadersEnabled = true;
                options.GeneratedCodeMode = TestEnvironment.IsTesting(configuration) ? TypeLoadMode.Dynamic : TypeLoadMode.Auto;
                options.AutoCreateSchemaObjects = AutoCreate.All;
                options.DisableNpgsqlLogging = true;
                options.UseNewtonsoftForSerialization(
                    nonPublicMembersStorage: NonPublicMembersStorage.NonPublicSetters,
                    enumStorage: EnumStorage.AsString,
                    configure: jsonOptions =>
                    {
                        jsonOptions.Converters.Add(new MartenUtcDateTimeOffsetConverter());
                        jsonOptions.Converters.Add(new MartenUtcDateTimeOffsetNullableConverter());
                    });

                options.Connection(connectionString);
                MartenConfigurationOrchestrator.RegisterMartenConfigurators(options, typeof(T).Assembly);
                MartenConfigurationOrchestrator.RegisterMartenEvents(options, typeof(T).Assembly);
            })
            .AddAsyncDaemon(DaemonMode.Solo);
        
        var providerType = Type.GetType("Marten.Linq.MartenLinqQueryProvider, Marten");
        CustomAsyncAdapters.RegisterAdapter(providerType, new MartenAsyncAdapter());
        
        return services;
    }
}