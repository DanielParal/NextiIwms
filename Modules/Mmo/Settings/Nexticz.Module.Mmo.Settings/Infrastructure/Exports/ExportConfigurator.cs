using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.ExportEntity;


namespace Nexticz.Module.Mmo.Settings.Infrastructure.Exports;

internal class ExportConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<ExportProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Export>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Export>());
    }
}