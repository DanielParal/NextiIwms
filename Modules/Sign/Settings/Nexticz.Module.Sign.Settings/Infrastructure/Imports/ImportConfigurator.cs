using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.Settings.Domain.ImportAggregate;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Imports;

internal class ImportConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<ImportProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Import>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Import>());
    }
}