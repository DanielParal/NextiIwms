using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.Settings.Domain.ExportAggregate;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Exports;

internal class ExportConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<ExportProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Export>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Export>());
    }
}