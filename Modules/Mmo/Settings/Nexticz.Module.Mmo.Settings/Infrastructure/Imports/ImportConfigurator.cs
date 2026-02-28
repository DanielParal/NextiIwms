using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;


namespace Nexticz.Module.Mmo.Settings.Infrastructure.Imports;

internal class ImportConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<ImportProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Import>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Import>());
    }
}