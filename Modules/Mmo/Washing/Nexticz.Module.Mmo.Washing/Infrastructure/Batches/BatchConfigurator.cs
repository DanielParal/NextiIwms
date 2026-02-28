using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.Batches;

internal class BatchConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<BatchProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Batch>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Batch>("es"));
    }
}