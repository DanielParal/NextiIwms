using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Infrastructure.Imports;

internal class ImportConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<ImportProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Import>().Index(x => x.Status);
        
        options.Schema.For<Import>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Import>());
    }
}